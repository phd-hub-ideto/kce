using AutoMapper;
using KhumaloCraft.Data.Sql;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain;
using KhumaloCraft.Integrations;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Reflection;

namespace KhumaloCraft.Application.Dependencies;

public static class GlobalDependencies
{
    public static void Register(Container container, EntryPoint entryPoint, IConfiguration configuration)
    {
        container.RegisterInstance(SettingsImpl.Configure(configuration));

        KhumaloCraftDependencies.Register(container, entryPoint);
        IntegrationsDependencies.Register(container, entryPoint);
        DomainDependencies.Register(container, entryPoint);
        SqlDependencies.Register(container, entryPoint);
        ApplicationDependencies.Register(container, entryPoint);

        var callingAssembly = Assembly.GetCallingAssembly(); // Must be fetched outside the lambda to be correct.

        // AutoMapper registration
        container.RegisterSingleton(() =>
        {
            var assemblies = new[]
            {
                callingAssembly,
                typeof(SqlDependencies).Assembly,
            };

            var mapperProvider = new MapperProvider(container, assemblies);
            return mapperProvider.GetMapper();
        });
    }

    public class MapperProvider
    {
        private readonly Container _container;
        private readonly Assembly[] _assemblies;

        public MapperProvider(Container container, Assembly[] assemblies)
        {
            _container = container;
            _assemblies = assemblies;
        }

        public IMapper GetMapper()
        {
            var mapperConfigurationExpression = new MapperConfigurationExpression();

            mapperConfigurationExpression.ConstructServicesUsing(_container.GetInstance);

            mapperConfigurationExpression.AddMaps(_assemblies);

            var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression);

            mapperConfiguration.AssertConfigurationIsValid();

            return new Mapper(mapperConfiguration, _container.GetInstance);
        }
    }

    public static Container ConfigureContainer(Action<Container, EntryPoint> registration, EntryPoint entryPoint)
    {
        var verifiedContainer = DependencyManager.ConfigureDefaultContainer((container) =>
        {
            switch (entryPoint)
            {
                case EntryPoint.Portal:
                case EntryPoint.Manage:
                case EntryPoint.ImageServer:
                    container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                    break;
                case EntryPoint.Services:
                    break;
                case EntryPoint.TaskExecute:
                case EntryPoint.TaskService:
                    container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                    break;
                default:
                    throw new NotImplementedException("Unknown entrypoint: " + entryPoint);
            }

            registration(container, entryPoint);
        });

        return verifiedContainer;
    }
}