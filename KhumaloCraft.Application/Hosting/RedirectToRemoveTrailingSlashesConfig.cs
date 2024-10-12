using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace KhumaloCraft.Application.Hosting;

public static class RedirectToRemoveTrailingSlashesConfig
{
    public static void Use(IApplicationBuilder app)
    {

        var options = new RewriteOptions()
            .AddRedirect("^(.*)/$", "$1");

        app.UseRewriter(options);
    }
}