using KhumaloCraft.Dependencies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SimpleInjector;

namespace KhumaloCraft.Application.Hosting;

public static class StaticFilesConfig
{
    public static void Use(IApplicationBuilder app, IWebHostEnvironment env, Container container, EntryPoint entryPoint, string staticFilesPath = null)
    {
        IFileProvider staticFileProvider = null;

        if (!string.IsNullOrEmpty(staticFilesPath))
        {
            staticFileProvider = new FilterFilesInRootFileProvider(env.ContentRootPath);

            env.WebRootPath = staticFilesPath;

            var fileProviders = new List<IFileProvider>()
            {
                env.WebRootFileProvider,
                staticFileProvider
            };

            env.WebRootFileProvider = new CompositeFileProvider(fileProviders);
        }

        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = env.WebRootFileProvider,
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365)
                };
                headers.Append(HeaderNames.Vary, HeaderNames.AcceptEncoding);
            }
        });
    }

    private class FilterFilesInRootFileProvider : IFileProvider
    {
        private readonly PhysicalFileProvider _contentRootFileProvider;

        public FilterFilesInRootFileProvider(string contentRootPath)
        {
            _contentRootFileProvider = new PhysicalFileProvider(contentRootPath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _contentRootFileProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!IsFileInRoot(subpath))
            {
                return _contentRootFileProvider.GetFileInfo(subpath);
            }

            return new NotFoundFileInfo(subpath);
        }

        private static bool IsFileInRoot(string subpath)
        {
            return Path.GetDirectoryName(subpath) == "\\";
        }

        public IChangeToken Watch(string filter)
        {
            return _contentRootFileProvider.Watch(filter);
        }
    }
}