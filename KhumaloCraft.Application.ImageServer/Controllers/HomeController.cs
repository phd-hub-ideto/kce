using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.ImageServer.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public IActionResult Index(Models.System.HomeModel model)
    {
        if (model.ImageId == null)
        {
            model.ImageId = 1;
            return View(model);

        }

        var imageId = model.ImageId.ToString();

        if (Imaging.ImageSizeOption.Original.ToString() == model.ImageSizeOption)
        {
            return Redirect($"/{imageId}");
        }

        return Redirect($"/{imageId}/{model.ImageSizeOption}");
    }
}