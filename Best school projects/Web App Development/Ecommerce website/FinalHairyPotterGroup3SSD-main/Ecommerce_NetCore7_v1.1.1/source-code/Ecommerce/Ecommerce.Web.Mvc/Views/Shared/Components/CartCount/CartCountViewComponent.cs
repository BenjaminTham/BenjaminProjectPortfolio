using Ecommerce.Application.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.CartCount;

public class CartCountViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cookieValueFromReq = Request.Cookies["shop-cart"]!;

        List<CartDto> cart = new List<CartDto>();
        if (cookieValueFromReq != null)
        {
            cart = JsonSerializer.Deserialize<List<CartDto>>(cookieValueFromReq);
        }

        var totalItem = cart.Select(c => c.Qty).Sum();
        ViewBag.CartItemCount = totalItem;
        return View(totalItem);
    }
}
