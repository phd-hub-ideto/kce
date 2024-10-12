using KhumaloCraft.Application.Portal.Models.Craftworks;
using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Security;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Controllers;

public sealed class MyWorkController(
    ICraftworkService craftworkService,
    IImageUrlBuilder imageUrlBuilder,
    ICartService cartService,
    IPrincipalResolver principalResolver) : Controller
{
    private readonly ICraftworkService _craftworkService = craftworkService;

    [Route("my-work")]
    public IActionResult Index()
    {
        Cart cart;

        if (principalResolver.IsAuthenticated())
        {
            cart = cartService.Fetch();
        }
        else
        {
            cart = new Cart();
        }

        var craftworks = _craftworkService.FetchAllCraftworks()
                            .Where(c => c.IsActive)
                            .Select(craft =>
                            {
                                return new CraftworkModel
                                {
                                    Id = craft.Id.Value,
                                    Title = craft.Title,
                                    Description = craft.Description,
                                    Price = craft.Price.ToStringWithDecimals(),
                                    ImageUrl = imageUrlBuilder.GetUrl(craft.PrimaryImageReferenceId),
                                    Category = craft.Category,
                                    Quantity = craft.Quantity,
                                };
                            });

        var model = CraftworkViewModel.Create(cart, craftworks);

        return View("MyWork", model);
    }
}