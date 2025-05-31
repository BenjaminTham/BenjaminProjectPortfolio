using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Categories.Queries;
using Ecommerce.Application.Handlers.ProductReviews.Queries;
using Ecommerce.Application.Handlers.Shop.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Mvc.Controllers;

[AllowAnonymous]
public class ShopController : Controller
{
    private readonly IMediator _mediator;
    public ShopController(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<IActionResult> Index(string? color, string? size, int? page, string sortColumn = "Id", string sortOrder = "Desc")
    {
        var getShopItems = await _mediator.Send(new GetShopWithPagingQuery { color = color, size = size, page = page, pageSize = 12, sortColumn = sortColumn, sortOrder = sortOrder });
        var productList = getShopItems?.PaginatedProductList?.Items;

        var availableColor = productList?.SelectMany(o => o.AvailableColorVariant).DistinctBy(o => o.Name).OrderBy(o => o.Name).ToList() ?? new List<ColorDto>();
        var availableSize = productList?.SelectMany(o => o.AvailableSizesVariant).DistinctBy(o => o.Name).ToList() ?? new List<SizeDto>();
        var availableCategory = await _mediator.Send(new GetCategoriesQuery());

        ViewBag.AvailableColor = availableColor;
        ViewBag.AvailableSize = availableSize;
        ViewBag.AvailableCategory = availableCategory.Where(o => o.ParentCategoryId == null).ToList();

        List<int> sizeList = size?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
        List<int> colorList = color?.Split(',').Select(int.Parse).ToList() ?? new List<int>(); ;

        ViewBag.SelectedColor = availableColor.Where(c => colorList.Contains(c.Id)).ToList();
        ViewBag.SelectedSize = availableSize.Where(c => sizeList.Contains(c.Id)).ToList();

        return View(getShopItems?.PaginatedProductList);
    }

    [Route("shop/{slug}")]
    public async Task<IActionResult> ByCategory(string slug, string? color, string? size, int? page, string sortColumn = "Id", string sortOrder = "Desc")
    {
        var selectedCategory = await _mediator.Send(new GetCategoryBySlugQuery { Slug = slug });
        var selectedCategoryList = await _mediator.Send(new GetAllChildrenCategoryByIdQuery { Id = selectedCategory.Id });

        string[] slugList = Array.Empty<string>();
        foreach (var result in ListFlatten(selectedCategoryList))
        {
            slugList = slugList.Append(result.Slug).ToArray();
        }

        var getShopItems = await _mediator.Send(new GetShopByCategoryWithPagingQuery { CategorySlug = slugList, color = color, size = size, page = page, pageSize = 9, sortColumn = sortColumn, sortOrder = sortOrder });
        var productList = getShopItems?.PaginatedProductList?.Items;

        var availableColor = productList?.SelectMany(o => o.AvailableColorVariant).DistinctBy(o => o.Name).OrderBy(o => o.Name).ToList() ?? new List<ColorDto>();
        var availableSize = productList?.SelectMany(o => o.AvailableSizesVariant).DistinctBy(o => o.Name).ToList() ?? new List<SizeDto>();
        var availableCategory = await _mediator.Send(new GetCategoriesQuery());

        ViewBag.SelectedCategory = selectedCategory;
        ViewBag.AvailableCategory = availableCategory.Where(c => c.Id == selectedCategory.Id).ToList()[0].Children;

        ViewBag.AvailableColor = availableColor;
        ViewBag.AvailableSize = availableSize;
        //ViewBag.AvailableCategory = availableCategory.Where(o => o.ParentCategoryId == null).ToList();

        List<int> sizeList = size?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
        List<int> colorList = color?.Split(',').Select(int.Parse).ToList() ?? new List<int>(); ;

        ViewBag.SelectedColor = availableColor.Where(c => colorList.Contains(c.Id)).ToList();
        ViewBag.SelectedSize = availableSize.Where(c => sizeList.Contains(c.Id)).ToList();

        return View(getShopItems?.PaginatedProductList);
    }

    public IEnumerable<CategoryDto> ListFlatten(CategoryDto dto)
    {
        yield return dto;

        foreach (var child in dto?.Children ?? Enumerable.Empty<CategoryDto>())
            foreach (var flattenedNode in ListFlatten(child))
                yield return flattenedNode;
    }


    [Route("productdetails/{slug}")]
    public async Task<IActionResult> ProductDetails(string slug)
    {
        var getProductDetails = await _mediator.Send(new GetProductDetailsBySlug { Slug = slug });
        var getProductReviews = await _mediator.Send(new GetCustomerReviewsByProductIdQuery { ProductId = getProductDetails.ProductDetails.Id });

        ViewData["SizeId"] = new SelectList(getProductDetails.AvailableSizes, "Id", "Name");
        ViewData["ColorId"] = new SelectList(getProductDetails.AvailableColors, "Id", "Name");

        ViewBag.SelectedCategorySlug = getProductDetails.ProductDetails.CategorySlug ?? "";
        ViewBag.ProductId = getProductDetails.ProductDetails.Id;
        ViewBag.ProductReviews = getProductReviews;
        return View(getProductDetails);
    }

    [Route("filterdetails/{id}")]
    public async Task<IActionResult> ProductDetailsFilter(int id, int color, int size)
    {
        var result = await _mediator.Send(new GetProductDetailsFilterResultQuery { Id = id, Color = color, Size = size });
        return Json(result);
    }

    [Route("quickview/{id}")]
    public IActionResult ProductQuickView(int id)
    {
        return ViewComponent("ProductQuickView", new { ProductId = id });
    }

}
