using KhumaloCraft.Domain.Images;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.ImageServer.Controllers;

public class WarmupController(IImageRepository imageRepository) : Controller
{
    [Route("admin/warmup")]
    public IActionResult Index()
    {
        var imageId = imageRepository.FetchOneImageId();

        var urls = new List<string>
        {
            "/",
            $"{imageId}",
        };

        return Content(string.Join(Environment.NewLine, urls), ContentType.Text);
    }
}