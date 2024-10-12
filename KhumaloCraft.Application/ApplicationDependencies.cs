using KhumaloCraft.Application.Authentication;
using KhumaloCraft.Application.Browsers;
using KhumaloCraft.Application.DisplayModes;
using KhumaloCraft.Application.Http;
using KhumaloCraft.Application.ImageHelpers;
using KhumaloCraft.Application.Monitoring;
using KhumaloCraft.Application.Routing.Routes;
using KhumaloCraft.Application.Session;
using KhumaloCraft.Application.Users;
using KhumaloCraft.Application.Users.LoginManager;
using KhumaloCraft.Application.Validation.User;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Http;
using SimpleInjector;

namespace KhumaloCraft.Application;

public static class ApplicationDependencies
{
    public static void Register(Container container, EntryPoint entryPoint)
    {
        container.RegisterUsingAttributes(typeof(ApplicationDependencies).Assembly);

        if (entryPoint == EntryPoint.TaskService || entryPoint == EntryPoint.TaskExecute)
        {
            container.RegisterSingleton<IHttpContextAccessor, NullHttpContextAccessor>();
            container.RegisterSingleton<IPrincipalResolver, DefaultPrincipalResolver>();
            container.RegisterSingleton<IHttpRequestProvider, NullHttpRequestProvider>();
            container.RegisterSingleton<IHttpResponseProvider, NullHttpResponseProvider>();
            container.RegisterSingleton<IHttpContextProvider, NullHttpContextProvider>();
        }
        else
        {
            container.RegisterSingleton<IPrincipalResolver, HttpContextPrincipalResolver>();
            container.RegisterSingleton<IHttpContextProvider, HttpContextProvider>();
            container.RegisterSingleton<IHttpRequestProvider, HttpRequestProvider>();
            container.RegisterSingleton<IHttpResponseProvider, HttpResponseProvider>();
            container.Register<IRoutedUrlBuilder, RoutedUrlBuilder>(Lifestyle.Singleton);
            container.Register<IKCRouteBuilder, RouteBuilder>(Lifestyle.Singleton);
        }

        container.RegisterSingleton<IBrowserDetector, BrowserDetector>();// http context provider
        container.RegisterSingleton<IDisplayModeSelector, DisplayModeSelector>();// http context provider
        container.RegisterSingleton<IHttpCookies, HttpCookies>();
        container.RegisterSingleton<IUniqueUserTracker, UniqueUserTracker>();
        container.RegisterSingleton<IRequestStorage, RequestStorage>();
        container.RegisterSingleton<UserProfileValidator>();
        container.RegisterSingleton<IImageUrlBuilder, ImageUrlBuilder>();
        container.RegisterSingleton<IImageDownloader, ImageDownloader>();
        container.RegisterSingleton<IUserActivationService, UserActivationService>();
        container.RegisterSingleton<IUserService, UserService>();
        container.RegisterSingleton<LoginManager>();
    }
}