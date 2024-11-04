using KhumaloCraft.Domain.SiteVisits;
using KhumaloCraft.Domain.UserEnvironment;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Builder;
using SimpleInjector;

namespace KhumaloCraft.Application.Hosting;

public static class SiteVisitLogMiddlewareConfig
{
    /// <summary>
    /// Logs visits to the site.
    /// </summary>
    /// <param name="applicationBuilder">The application builder.</param>
    /// <param name="container">The Simple Injector container.</param>
    public static void Use(IApplicationBuilder applicationBuilder, Container container)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            var response = context.Response;

            response.OnStarting(() =>
            {
                try
                {
                    var siteVisitLogRepository = container.GetInstance<ISiteVisitLogRepository>();
                    var environment = container.GetInstance<IEnvironmentService>().GetEnvironment();

                    //TODO-LP : Also log the payload
                    var siteVisitLog = new SiteVisitLog
                    {
                        RequestPath = context.Request.Path,
                        ContentType = response.ContentType ?? context.Request.Headers.ContentType,
                        Scheme = context.Request.Scheme,
                        Referrer = context.Request.Headers.Referer,
                        Method = context.Request.Method,
                        Host = context.Request.Headers.Host.FirstOrDefault() ?? response.Headers.Host,
                        StatusCode = response.StatusCode,
                        Location = response.Headers.Location,
                        UserAgent = context.Request.Headers.UserAgent,
                        Platform = environment.Platform.GetBestDescription(),
                        IpAddress = environment.IPAddress.ToString(),
                        UserId = environment.UserId,
                        UniqueUserId = environment.UniqueUserId,
                    };

                    siteVisitLogRepository.Insert(siteVisitLog);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Logging Site Visit: {ex.Message}");
                }

                return Task.CompletedTask;
            });

            await next(context);

        });
    }
}