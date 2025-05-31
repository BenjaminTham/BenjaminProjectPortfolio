using AutoMapper;
using Ecommerce.Application.Handlers.ProductReviews.Commands;
using Ecommerce.Application.Handlers.ProductReviews.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Extension;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class CustomerReviewController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public CustomerReviewController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    [Authorize(Permissions.Permissions_ProductReview_View)]
    public IActionResult Index()
    {
        return View();
    }
    [Authorize(Permissions.Permissions_ProductReview_View)]
    [HttpPost]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().PostPageResponse(Request);
        var result = await _mediator.Send(new GetCustomerReviewsWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    [Authorize(Permissions.Permissions_ProductReview_Edit)]
    public async Task<IActionResult> Details(int id)
    {
        var orderDetails = await _mediator.Send(new GetCustomerReviewDetailsByIdQuery { ReviewId = id });
        return View(orderDetails);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_ProductReview_Edit)]
    public async Task<IActionResult> AddReply(CreateReplyForCustomerReviewCommand command)
    {

        if (!ModelState.IsValid) return RedirectToAction("Details", new { id = command.Id });
        
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction("Details", new { id = command.Id });
        }

        return RedirectToAction("Details", new { id = command.Id });
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_ProductReview_Edit)]
    public async Task<IActionResult> UpdateProductReviewVisibility(UpdateProductReviewVisibilityCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return Json(new JsonResponse { Success = true, Data = response });
        }
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }



}
