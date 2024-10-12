using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Models.Craftworks;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Application.Routing.Routes;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Structs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public sealed class CraftworkController(
    IKCRouteBuilder kCRouteBuilder,
    IImageUrlBuilder imageUrlBuilder,
    ICraftworkService craftworkService) : BaseController
{
    [HttpGet]
    [Route("craftworks", Name = RouteNames.Craftwork.Craftworks)]
    public ActionResult Craftworks()
    {
        if (!HasAccess(AdministratorPermission.ManageProduct))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var craftworks = craftworkService.FetchAllCraftworks()
                                .OrderByDescending(c => c.Id)
                                .Select(i => CraftworkGridItem.Create(i, Url))
                                .ToList();

        return View(craftworks);
    }

    [HttpGet]
    [Route("create-craftwork", Name = RouteNames.Craftwork.CreateCraftwork)]
    public ActionResult CreateCraftwork()
    {
        if (!HasAccess(AdministratorPermission.ManageProduct))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        return View();
    }

    [HttpPost]
    [Route("create-craftwork", Name = RouteNames.Craftwork.CreateCraftworkPost)]
    public ActionResult CreateCraftworkPost([FromForm] CreateCraftworkModel model)
    {
        if (!HasAccess(AdministratorPermission.ManageProduct))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (ModelState.IsValid)
        {
            using var scope = new TransactionScope();

            var craftwork = new Craftwork
            {
                Category = model.Category,
                Title = model.Title,
                Description = model.Description,
                Price = new Money(model.Price),
                IsActive = model.IsActive,
                PrimaryImageReferenceId = model.ImageReferenceId.Value,
                Quantity = model.Quantity.Value
            };

            craftworkService.AddCraftwork(craftwork);

            scope.Complete();

            return Redirect(Url.Action<CraftworkController>(c => c.ViewCraftwork(craftwork.Id.Value)));
        }

        return View("CreateCraftwork", model);
    }

    [HttpGet]
    [Route("craftwork/{id:int}", Name = RouteNames.Craftwork.ViewCraftwork)]
    public ActionResult ViewCraftwork(int id)
    {
        if (!HasAccess(AdministratorPermission.ManageProduct))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (!craftworkService.TryFetchByCraftworkId(id, out var craftwork))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var model = ViewCraftworkModel.Create(craftwork, imageUrlBuilder);

        return View(model);
    }
}