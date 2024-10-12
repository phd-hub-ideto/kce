using Microsoft.AspNetCore.Builder;
using SimpleInjector;

namespace KhumaloCraft.Application.Hosting;

public static class ReturnUrlValidationMiddlewareConfig
{
    /// <summary>
    /// Validates URL parameters in the request and redirects to the homepage if the parameters contain invalid URLs.
    /// </summary>
    /// <param name="applicationBuilder">The application builder.</param>
    /// <param name="container">The Simple Injector container.</param>
    public static void Use(IApplicationBuilder applicationBuilder, Container container)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            //https://www.khumalocraft.co.za///openbugbountry.org/ multiple forward slashes on the path (3 /// and above) forces a redirect to OBB
            if (context.Request.Path.Value.StartsWith("///"))
            {
                context.Response.Redirect("/", true);

                return; //If not returned, it ignores home page redirect and it redirects to OBB
            }

            var queryParameters = context.Request.Query;

            foreach (var parameter in queryParameters)
            {
                if (IsAbsoluteUrl(parameter.Value) && !IsValidDomain(container, parameter.Value.ToString()))
                {
                    context.Response.Redirect("/", true);

                    return;
                }
            }

            await next(context);

        });
    }

    private static bool IsAbsoluteUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    private static bool IsValidDomain(Container container, string uri)
    {
        var settings = container.GetInstance<ISettings>();

        var referenceUrls = new[] { settings.PortalBaseUri };

        var parsedUris = referenceUrls.Select(x => new Uri(x));

        var allowedDomains = parsedUris.Select(x => x.Host).ToHashSet();

        if (uri.IsNullOrEmpty()) return true;

        if (Uri.TryCreate(uri, UriKind.Absolute, out var absoluteUri))
        {
            var scheme = absoluteUri.Scheme;

            if (scheme is not "https" || absoluteUri.PathAndQuery.StartsWith("///")) return false;

            return allowedDomains.Contains(absoluteUri.Host);
        }

        return false;
    }
}