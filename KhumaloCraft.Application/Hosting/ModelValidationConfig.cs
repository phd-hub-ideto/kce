using KhumaloCraft.Application.Validation;
using KhumaloCraft.Dependencies;
using Microsoft.AspNetCore.Mvc;
using SimpleInjector;
using System.Reflection;

namespace KhumaloCraft.Application.Hosting;

public static class ModelValidationConfig
{
    public static void Register(Container container)
    {
        var iValidatorType = typeof(IValidator<>);
        container.Register(typeof(ModelValidator<>), typeof(ModelValidator<>), Lifestyle.Singleton);
        container.RegisterSingletonCollectionFromAssembly(iValidatorType, Assembly.GetCallingAssembly());
        container.RegisterSingletonCollectionFromAssembly(iValidatorType, Assembly.GetAssembly(iValidatorType));
    }

    public static void Add(MvcOptions options, Container container)
    {
        options.ModelValidatorProviders.Add(new SimpleInjectorModelValidatorProvider(container));
    }
}