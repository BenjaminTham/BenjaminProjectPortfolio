using AutoMapper;
using Ecommerce.Application.Handlers.Cart.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[AllowAnonymous]
public class CartController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public CartController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("addtocart")]
    public async Task<IActionResult> AddToCart(AddToCartCommand addToCart)
    {
        var getProductDetails = await _mediator.Send(addToCart);
        return Json("");
    }


    [HttpGet]
    [Route("getcartcount")]
    public IActionResult GetCartItemCount()
    {
        return ViewComponent("CartCount");
    }

    [HttpGet]
    [Route("getcartsummary")]
    public IActionResult GetCartSummary()
    {
        return ViewComponent("CartSummary");
    }

    [HttpGet]
    [Route("getcart")]
    public IActionResult GetCart()
    {
        return ViewComponent("Cart");
    }

    [HttpGet]
    [Route("getcheckoutorderpreview")]
    public IActionResult GetCheckoutOrderPreview()
    {
        return ViewComponent("CheckoutOrderPreview");
    }

    [HttpGet]
    [Route("cartitemincrement/{variant}")]
    public async Task<IActionResult> CartItemIncremenet(int variant)
    {
        var getProductDetails = await _mediator.Send(new IncremenetCartItemCommand { VariantId = variant });
        return Json("");
    }

    [HttpGet]
    [Route("cartitemdecrement/{variant}")]
    public async Task<IActionResult> CartItemDecrement(int variant)
    {
        var getProductDetails = await _mediator.Send(new DecrementCartItemCommand { VariantId = variant });
        return Json("");
    }

    [HttpGet]
    [Route("cartitemremove/{variant}")]
    public async Task<IActionResult> CartItemDelete(int variant)
    {
        var getProductDetails = await _mediator.Send(new DeleteCartItemCommand { VariantId = variant });
        return Json("");
    }
}
