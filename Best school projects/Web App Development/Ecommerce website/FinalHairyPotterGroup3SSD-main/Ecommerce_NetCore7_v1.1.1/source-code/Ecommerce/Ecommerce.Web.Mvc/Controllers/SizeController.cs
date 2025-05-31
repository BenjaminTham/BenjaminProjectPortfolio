using AutoMapper;
using Ecommerce.Application.Handlers.Colors.Queries;
using Ecommerce.Application.Handlers.Sizes.Commands;
using Ecommerce.Application.Handlers.Sizes.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class SizeController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public SizeController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Permissions.Permissions_Size_View)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Permissions.Permissions_Size_View)]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetSizesWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    [Authorize(Permissions.Permissions_Size_Create)]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Permissions.Permissions_Size_Create)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSizeCommand command)
    {

        if (!ModelState.IsValid) return View(command);

        var isSizeExists = await _mediator.Send(new IsSizeNameExistQuery { Name = command.Name });
        if (isSizeExists) ModelState.AddModelError(string.Empty, "Size already exist.");

        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction(nameof(Index));
        }

        return View(command);
    }

    [Authorize(Permissions.Permissions_Size_Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var size = await _mediator.Send(new GetSizeByIdQuery { Id = id });
        if (size == null)
        {
            return NotFound();
        }

        var sizeCommand = _mapper.Map<UpdateSizeCommand>(size);
        return View(sizeCommand);
    }

    [Authorize(Permissions.Permissions_Size_Edit)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSizeCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        return View(command);
    }

    [Authorize(Permissions.Permissions_Size_Delete)]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteSizeCommand { Id = id });
        return Json(response);
    }
}
