using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Hubs;
using KhumaloCraft.Application.Portal.Models.Craftworks;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Application.Routing.Routes;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Dates;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Structs;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public sealed class CraftworkController(
    IKCRouteBuilder kCRouteBuilder,
    IImageUrlBuilder imageUrlBuilder,
    ICraftworkService craftworkService,
    INotificationsService notificationsService,
    IDateProvider dateProvider) : BaseController
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

            return Redirect(Url.Action<CraftworkController>(c => c.Craftwork(craftwork.Id.Value)));
        }

        return View("CreateCraftwork", model);
    }

    [HttpGet]
    [Route("craftwork/{id:int}", Name = RouteNames.Craftwork.ViewCraftwork)]
    public ActionResult Craftwork(int id)
    {
        if (!HasAccess(AdministratorPermission.ManageProduct))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (!craftworkService.TryFetchByCraftworkId(id, out var craftwork))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var model = UpdateCraftworkModel.Create(craftwork, imageUrlBuilder);

        return View(model);
    }

    [HttpPost]
    [Route("craftwork/{id:int}", Name = RouteNames.Craftwork.UpdateCraftwork)]
    public async Task<ActionResult> UpdateCraftwork([FromRoute] int id, [FromForm] UpdateCraftworkModel model)
    {
        if (!HasAccess(AdministratorPermission.ManageProduct))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (!craftworkService.TryFetchByCraftworkId(id, out var craftwork))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (ModelState.IsValid)
        {
            var priceIncreased = model.Price > craftwork.Price.Amount;
            var priceReduced = model.Price < craftwork.Price.Amount;
            var discontinued = craftwork.IsActive != model.IsActive && !model.IsActive;
            var outOfStock = craftwork.Quantity != model.Quantity && model.Quantity == 0;
            var inStock = craftwork.Quantity != model.Quantity && craftwork.Quantity == 0;

            var oldPrice = craftwork.Price;

            using var scope = new TransactionScope();

            craftwork.Title = model.Title;
            craftwork.Description = model.Description;
            craftwork.Category = model.Category;
            craftwork.Price = new Money(model.Price);
            craftwork.Quantity = model.Quantity.Value;
            craftwork.IsActive = model.IsActive;
            craftwork.PrimaryImageReferenceId = model.ImageReferenceId.Value;

            craftworkService.UpdateCraftwork(craftwork);

            if (priceIncreased)
            {
                await notificationsService.SendNotification(NotificationType.ItemPriceIncreased, craftwork.Id.Value);
            }

            if (priceReduced)
            {
                await notificationsService.SendNotification(NotificationType.ItemPriceReduced, craftwork.Id.Value, oldPrice: oldPrice);
            }

            if (discontinued)
            {
                await notificationsService.SendNotification(NotificationType.ItemDiscontinued, craftwork.Id.Value);
            }

            if (outOfStock)
            {
                await notificationsService.SendNotification(NotificationType.ItemOutOfStock, craftwork.Id.Value);
            }

            if (inStock)
            {
                await notificationsService.SendNotification(NotificationType.ItemInStock, craftwork.Id.Value);
            }

            scope.Complete();

            ViewBag.SuccessMessage = "Craftwork Updated Successfully!";
        }

        model.Id = id;
        model.UpdatedDate = FormattingHelper.FormatDateTime(dateProvider.GetDate());

        model.SetImageUrl(imageUrlBuilder);

        return View("Craftwork", model);
    }
}