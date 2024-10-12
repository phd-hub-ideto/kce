using KhumaloCraft.Application.Portal.Models.Cart;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Domain.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public sealed class CartController(
    ICartService cartService,
    ISettings settings) : BaseController
{
    [HttpGet]
    [Route("cart", Name = RouteNames.Cart.ViewCart)]
    public ActionResult Cart()
    {
        var cart = cartService.Fetch();

        var cartSummary = CartSummaryModel.Create(cart, settings);

        return View(cartSummary);
    }

    [HttpPost]
    [Route("cart/add-item", Name = RouteNames.Cart.AddToCart)]
    public JsonResult AddToCart([FromBody] AddToCartModel model)
    {
        if (ModelState.IsValid &&
            cartService.AddItemToCart(model.CraftworkId.Value))
        {
            return Json(new { success = true });
        }

        return Json(new { success = false, message = "Unexpected Error Occurred!" });
    }

    [HttpPost]
    [Route("cart/increment-cart-item", Name = RouteNames.Cart.IncrementCartItem)]
    public JsonResult IncrementCartItem([FromBody] UpdateCartModel model)
    {
        if (ModelState.IsValid &&
            cartService.IncrementCartItem(model.CraftworkId.Value, out var quantity))
        {
            return Json(new { success = true, quantity });
        }

        return Json(new { success = false, message = "Unexpected Error Occurred!" });
    }

    [HttpPost]
    [Route("cart/decrement-cart-item", Name = RouteNames.Cart.DecrementCartItem)]
    public JsonResult DecrementCartItem([FromBody] UpdateCartModel model)
    {
        if (ModelState.IsValid &&
            cartService.DecrementCartItem(model.CraftworkId.Value, out var quantity))
        {
            return Json(new { success = true, quantity });
        }

        return Json(new { success = false, message = "Unexpected Error Occurred!" });
    }
}