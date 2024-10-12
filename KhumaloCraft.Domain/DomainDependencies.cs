using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Events;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Team;
using SimpleInjector;

namespace KhumaloCraft.Domain;

public static class DomainDependencies
{
    public static void Register(Container container, EntryPoint entryPoint)
    {
        container.RegisterUsingAttributes(typeof(DomainDependencies).Assembly);

        container.RegisterSingleton<ITeamService, TeamService>();
        container.RegisterSingleton<ICraftworkService, CraftworkService>();
        container.RegisterSingleton<IPermissionSource, CurrentPermissionScopePermissionSource>();
        container.RegisterInstance<IPermissionService>(PermissionManager.Current);

        container.RegisterSingletonCollectionFromAssembly(typeof(IEventHandler<>), typeof(IEventHandler<>).Assembly);

        container.RegisterSingletonCollectionFromAssembly(typeof(FluentValidation.IValidator<>), typeof(DomainDependencies).Assembly);
    }
}