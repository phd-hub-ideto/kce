using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KhumaloCraft.Application.ImageServer.Controllers;

public class ImageController(
    IImageDataRepository imageDataRepository,
    IImageRepository imageRepository) : Controller
{
    //TODO-LP : Implement ImageResizer
    //private readonly ImageResizer _imageResizer;

    [Route("{imageId:int}")]
    [Route("{imageId:int}/{sizeOption}")]
    public async Task<IActionResult> Home(int imageId, ImageSizeOption? sizeOption)
    {
        if (Request.GetTypedHeaders().LastModified.HasValue) // Cached on Client
        {
            return StatusCode((int)HttpStatusCode.NotModified);
        }

        var (result, imageData) = await imageDataRepository.TryGetAsync(new ImageId(imageId)).ConfigureAwait(false);

        if (!result)
        {
            (result, imageData) = await imageRepository.TryGetImageByImageIdAsync(new ImageId(imageId)).ConfigureAwait(false);
        }

        if (result)
        {
            return new ImageActionResult(imageData);
        }

        return new StatusCodeWithReasonResult(HttpStatusCode.NotFound, "Image or Image Data not found.");
    }
}