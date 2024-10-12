using KhumaloCraft.Domain.Images;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.ImageServer.Controllers;

public class HealthCheckController(IImageRepository imageRepository) : Controller
{
    [Route("health/image")]
    public IActionResult Index()
    {
        var imageId = imageRepository.FetchOneImageId();

        return Redirect(Url.Action("Home", "Image", new { imageId = imageId.Value }));
    }
}