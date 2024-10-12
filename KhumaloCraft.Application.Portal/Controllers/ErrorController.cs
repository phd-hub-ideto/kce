using System.Net;
using KhumaloCraft.Application.DisplayModes;
using KhumaloCraft.Application.Portal.Models.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Controllers;

[DesktopDisplayModeOnly]
public class ErrorController : BaseController
{
    [Route("errors/throw")]
    public ActionResult Throw()
    {
        throw new Exception("This is a test exception.");
    }

    [Route("errors/400")]
    public new ActionResult BadRequest()
    {
        return View();
    }

    [AllowAnonymous]
    [Route("errors/403")]
    public ActionResult Forbidden(string errorReference)
    {
        return View(ErrorModel.Create(errorReference));
    }

    [AllowAnonymous]
    [SupportedDisplayModes(DisplayModeType.Desktop, DisplayModeType.SmartPhone)]
    [Route("errors/404")]
    public new ActionResult NotFound()
    {
        return CreateErrorPage(HttpStatusCode.NotFound, "Not Found");
    }

    [AllowAnonymous]
    [SupportedDisplayModes(DisplayModeType.Desktop, DisplayModeType.SmartPhone)]
    [Route("errors/500")]
    public ActionResult ServerError(string errorReference)
    {
        return CreateErrorPage(HttpStatusCode.InternalServerError, errorReference);
    }
}