using KhumaloCraft.Helpers;
using KhumaloCraft.Http;
using SimpleInjector;

namespace KhumaloCraft.Dependencies;

public static class DependencyManager
{
    private static Container _container;

    //These have to remain static properties until such a time that we have removed statically constructed classes
    // or statically configured instances, such as ConfigurationStatic and SettingsImpl. Unfortunately due to dependencies
    // we cannot inject these as the DI container has not been configured when they need to be accessed in said static constructors
    // or configure methods.
    public static IHttpContextProvider HttpContextProvider => IsConfigured ? Container.GetInstance<IHttpContextProvider>() : new NullHttpContextProvider();
    public static IHttpRequestProvider HttpRequestProvider => IsConfigured ? Container.GetInstance<IHttpRequestProvider>() : new NullHttpRequestProvider();
    public static IHttpResponseProvider HttpResponseProvider => IsConfigured ? Container.GetInstance<IHttpResponseProvider>() : new NullHttpResponseProvider();

    public static Container Container
    {
        get { return _container ?? throw new InvalidOperationException("Container has not been configured"); }
    }

    public static bool IsConfigured => _container is not null;

    public static Lifestyle DetermineLifestyle(EntryPoint entryPoint)
    {
        return entryPoint.In(EntryPoint.TaskService, EntryPoint.TaskExecute)
            ? Lifestyle.Scoped
            : Lifestyle.Singleton;
    }

    public static Container ConfigureDefaultContainer(Action<Container> registration, bool verify = true)
    {
        var container = new Container();

        registration(container);

        if (verify)
        {
            container.Verify();
        }

        return _container = container;
    }
}