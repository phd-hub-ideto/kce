using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Http;
using KhumaloCraft.Imaging;
using KhumaloCraft.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public class ImagesController : BaseController
{
    private readonly IImageService _imageService;
    private readonly IImageReferenceManager _imageReferenceManager;

    public ImagesController(
        IImageService imageService,
        IImageReferenceManager imageReferenceManager)
    {
        _imageService = imageService;
        _imageReferenceManager = imageReferenceManager;
    }

    [HttpPost]
    public async Task<ActionResult> UploadImage([FromQuery] uint? width, [FromQuery] uint? height, [FromQuery] ImageResizeOption? resizeOption)
    {
        ImageUpload imageUpload = await GetImageAsync(Request, width, height, resizeOption);

        var result = _imageService.AddImage(imageUpload, null);

        var url = ActionHelper.Action<ReadOnlyImagesController>(Url, c => c.FetchImage(result.ReferenceId, width, height, resizeOption));
        var thumbnail = ActionHelper.Action<ReadOnlyImagesController>(Url, c => c.FetchImage(result.ReferenceId, ImageManager.TileSize.Width, ImageManager.TileSize.Height, resizeOption));

        return Json(new { id = result.ReferenceId, url, thumbnail, });
    }

    private async Task<ImageUpload> GetImageAsync(HttpRequest request, uint? width, uint? height, ImageResizeOption? resizeOption, DateTime? modifiedDate = null)
    {
        using var memoryStream = new MemoryStream();

        await request.Form.Files[0].CopyToAsync(memoryStream);

        var bytes = memoryStream.ToArray();

        byte[] data;

        if (resizeOption == null)
        {
            data = bytes;
        }
        else
        {
            var imageSize = ImageResizeOption.EnsurePanoramic == resizeOption ? ImageDimensions.FourTimesMaximum : ImageDimensions.Maximum;

            if (width == null || width.Value > imageSize.Width)
                width = imageSize.Width;
            if (height == null || height.Value > imageSize.Height)
                height = imageSize.Height;

            data = ImagingUtilities.EnsureImage(bytes, width.Value, height.Value, resizeOption);
        }

        return new ImageUpload(data, modifiedDate ?? DateTime.Now);
    }

    private bool TryValidateImageSize(ImageUpload imageUpload, out string errorMessage)
    {
        var imageSizeMB = imageUpload.Data.LongLength / Math.Pow(1024, 2);

        if (imageSizeMB > ImageDataSizeLimits.MaximumImageSizeMB)
        {
            errorMessage = $"Image size of {Math.Round(imageSizeMB, 2)} MB is larger than the maximum allowed {ImageDataSizeLimits.MaximumImageSizeMB} MB";
            return false;
        }

        errorMessage = default;
        return true;
    }
}

public static class ImageDimensions
{
    public static readonly ImageSize Minimum = new ImageSize(0, 0);

    // Full HD
    public static readonly ImageSize Maximum = new ImageSize(1920, 1080);

    // 3840x2160; 4x full HD; 4K
    public static readonly ImageSize FourTimesMaximum = new ImageSize(Maximum.Width * 2, Maximum.Height * 2);
}

internal static class ImageManager
{
    public static readonly ImageSize TileSize = new ImageSize(508, 373);
}

public class ReadOnlyImagesController : ImagesController
{
    private readonly IImageService _imageService;

    public ReadOnlyImagesController(
        IImageService imageService,
        IImageReferenceManager imageReferenceManager) :
        base(imageService, imageReferenceManager)
    {
        _imageService = imageService;
    }

    private class ImageResult : ActionResult
    {
        public ImageResult(ImageContent image, uint? width = null, uint? height = null, ImageResizeOption? resizeOption = null)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            Image = image;
            Width = width;
            Height = height;
            ResizeOption = resizeOption;
        }

        public uint? Height { get; private set; }

        public uint? Width { get; private set; }

        public ImageResizeOption? ResizeOption { get; private set; }

        public ImageContent Image { get; private set; }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            if (Image == null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }
            var request = context.HttpContext.Request;
            var header = request.Headers["if-modified-since"];
            if (header.Count > 0)
            {

                //TODO-LP : Implement accurate DateTimeUtils
                /*if (DateTimeUtils.ParseHttpDate(header, out DateTime ifModifiedSince) && DateTimeUtils.SecondsOnlyPrescision.Equals(Image.ModifiedDate, ifModifiedSince))
                {
                    response.StatusCode = (int)HttpStatusCode.NotModified;
                    await response.CompleteAsync();
                }*/

                if (DateTime.TryParse(header, out DateTime ifModifiedSince) && SecondsOnlyPrescision.Equals(Image.ModifiedDate, ifModifiedSince))
                {
                    response.StatusCode = (int)HttpStatusCode.NotModified;

                    await response.CompleteAsync();
                }
            }

            var result = Image.Data;

            if ((Width != null) && (Height != null))
            {
                result = ImagingUtilities.EnsureImage(result, Width.Value, Height.Value, ResizeOption);
            }

            response.ContentType = Image.ContentType.ToMimeName();
            response.Headers.Append("content-length", result.Length.ToString());
            response.SetLastModified(Image.ModifiedDate ?? DateTime.Now);
            response.SetExpires(DateTime.MaxValue);
            await response.Body.WriteAsync(result);
        }
    }

    public ActionResult ImageNotFound()
    {
        return StatusCode((int)HttpStatusCode.NotFound, "Image not found.");
    }

    [HttpGet]
    public ActionResult FetchImage(int? id, uint? width, uint? height, ImageResizeOption? resizeOption)
    {
        return FetchImageShared(id, width, height, resizeOption);
    }

    private ActionResult FetchImageShared(int? id, uint? width, uint? height, ImageResizeOption? resizeOption)
    {
        if (id == null)
        {
            return ImageNotFound();
        }
        var image = _imageService.FetchImage(id.Value);

        return new ImageResult(image, width, height, resizeOption);
    }
}