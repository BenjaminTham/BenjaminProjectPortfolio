using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Categories.Queries;
using Ecommerce.Application.Handlers.Colors.Queries;
using Ecommerce.Application.Handlers.Products.Commands;
using Ecommerce.Application.Handlers.Products.Queries;
using Ecommerce.Application.Handlers.Sizes.Queries;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using Ecommerce.Web.Mvc.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMediator mediator, ILogger<ProductController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Authorize(Permissions.Permissions_Product_View)]
    public IActionResult Index()
    {
        return View();
    }
    [Authorize(Permissions.Permissions_Product_View)]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetProductsWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    [Authorize(Permissions.Permissions_Product_Create)]
    public async Task<IActionResult> Create()
    {
        ViewData["CategoryId"] = new SelectList(await _mediator.Send(new GetCategoriesQuery()), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_Product_Create)]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        ViewData["CategoryId"] = new SelectList(await _mediator.Send(new GetCategoriesQuery()), "Id", "Name");
        var isSlugExists = await _mediator.Send(new IsProductSlugExistQuery { Slug = command.Slug });
        if(isSlugExists) ModelState.AddModelError(string.Empty, "Slug already exist.");

        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, response.Message);
        }
        //return Json(new JsonResponse { Success = false, Error = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray() });

        return View(command);
    }

    [Authorize(Permissions.Permissions_Product_Edit)]
    public async Task<IActionResult> Details(int id)
    {
        var response = await _mediator.Send(new GetProductWithVariablesByIdQuery { Id = id });

        var productVariantTheme = typeof(ProductVariantTheme).GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(o => new { Text = o.GetValue(null).ToString(), Value = o.Name }).ToList();

        if (response == null) return NotFound();

        var size = await _mediator.Send(new GetSizesQuery());
        var color = await _mediator.Send(new GetColorsQuery());
        ViewData["CategoryId"] = new SelectList(await _mediator.Send(new GetCategoriesQuery()), "Id", "Name", response.CategoryId);             
        ViewData["SizeId"] = new SelectList(size.Where(c=>c.IsActive), "Id", "Name");
        ViewData["ColorId"] = new SelectList(color.Where(c => c.IsActive), "Id", "Name");
        ViewData["VariableTheme"] = new SelectList(productVariantTheme, "Text", "Text", response.VariableTheme);
        return View(response);
    }

    public async Task<IActionResult> ProductVariablesById(int id)
    {
        var response = await _mediator.Send(new GetProductVariablesByIdQuery { Id = id });
        return Json(response);
    }


    [HttpPost]
    [Authorize(Permissions.Permissions_Product_Edit)]
    public async Task<IActionResult> UpdateProduct(ProductForEditDto data)
    {
        // General regex pattern for most fields
        var regexPattern = @"^[a-zA-Z0-9\s.,!?#_\[\]""-]*$";
        // Custom regex pattern for Description to allow HTML but prevent <script> tags
        var descriptionPattern = @"^(?!.*<script>).*";

        _logger.LogInformation("Starting update for product with ID: {ProductId}", data.ProductId);

        //continue below
        if (data != null)
        {
            StringBuilder errorFields = new StringBuilder();

            // Helper function to check and log field errors
            void CheckField(string fieldName, string fieldValue, string pattern)
            {
                if (!Regex.IsMatch(fieldValue ?? "", pattern))
                {
                    errorFields.AppendLine($"{fieldName}: contains invalid content");
                }
            }

            // Validate each property that requires regex validation
            CheckField("Name", data.Name, regexPattern);
            CheckField("Slug", data.Slug, regexPattern);
            CheckField("ShortDescription", data.ShortDescription, regexPattern);
            CheckField("KeySpecs", data.KeySpecs, regexPattern);
            // Custom validation for Description
            CheckField("Description", data.Description, descriptionPattern);

            foreach (var variant in data.ProductVariant)
            {
                CheckField($"Variant Title ({variant.Title})", variant.Title, regexPattern);
                CheckField($"Variant SKU ({variant.Sku})", variant.Sku, regexPattern);
                CheckField($"Variant Slug ({variant.Slug})", variant.Slug, regexPattern);
            }

            if (errorFields.Length > 0)
            {
                _logger.LogWarning("Validation failed for product with ID: {ProductId}. Invalid fields: {Fields}", data.ProductId, errorFields.ToString());
                ModelState.AddModelError("RegexValidation", "One or more fields contain invalid characters or content.");
                return Json(new { success = false, message = "Validation failed, some fields contain invalid characters or content.", errors = errorFields.ToString() });
            }

            var response = await _mediator.Send(new UpdateProductWithVariablesCommand { ProductForEditDto = data });
            if (response.Succeeded)
            {
                _logger.LogInformation("Product with ID: {ProductId} was updated successfully", data.ProductId);
                return Json(response.Data);
            }
            _logger.LogError("Failed to update product with ID: {ProductId}", data.ProductId);
            return Json(new { success = false, message = "Failed to update product." });
        }

        _logger.LogError("Update attempted with null data for product with ID: {ProductId}", data?.ProductId);
        return Json(new { success = false, message = "Data is null, update failed." });
    }




    [AllowAnonymous]
    public async Task<IActionResult> GetSize()
    {
        var res = await _mediator.Send(new GetSizesQuery());
        return Json(res.Where(c => c.IsActive).Select(o => new DropdownSelectVM { Value = o.Id, Text = o.Name }));
    }

    [AllowAnonymous]
    public async Task<IActionResult> GetColor()
    {
        var res = await _mediator.Send(new GetColorsQuery());
        return Json(res.Where(c => c.IsActive).Select(o => new DropdownSelectVM { Value = o.Id, Text = o.Name }));
    }




    [AllowAnonymous]
    [Route("[controller]/[action]/{searchValue}")]
    [HttpGet]
    public async Task<IActionResult> ProductSearch(string searchValue)
    {
        var response = await _mediator.Send(new GetProductsBySearchQuery { SearchValue = searchValue, MaxResult = 8 });
        return Json(response);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Product_Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteProductCommand { Id = id });
        return Json(response);
    }
}
