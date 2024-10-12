using KhumaloCraft.Application.ImageServer.Helpers;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace KhumaloCraft.Application.ImageServer;

public class ImageActionResult(ImageData imageData) : IActionResult
{
    private static readonly DateTime _forever = new DateTime(2048, 12, 31);

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;

        var responseHeaders = response.GetTypedHeaders();

        responseHeaders.CacheControl = new CacheControlHeaderValue()
        {
            Public = true,
            MustRevalidate = false,
        };

        responseHeaders.Expires = _forever;
        responseHeaders.LastModified = GeneralHelpers.Minimum(imageData.ModifiedDate, DateTime.Now);
        responseHeaders.ContentType = new MediaTypeHeaderValue(imageData.ContentType.ToMimeName());

        response.Headers.Remove("Set-Cookie");

        await response.BodyWriter.WriteAsync(imageData.Data);
    }
}