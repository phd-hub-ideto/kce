using KhumaloCraft.Application.Http;
using KhumaloCraft.Application.Platforms;
using KhumaloCraft.Application.Portal.PlatformDetermination;
using KhumaloCraft.Dependencies;
using SimpleInjector;

namespace KhumaloCraft.Application.Portal;

public static class PortalDependencies
{
    public static void Register(Container container)
    {
        var currentAssembly = typeof(PortalDependencies).Assembly;

        container.RegisterUsingAttributes(currentAssembly);

        container.RegisterSingleton<IPlatformDeterminator, PortalPlatformDeterminator>();
        container.RegisterSingleton<ITemplateNameProvider, TemplateNameProvider>();
    }
}