using AutoMapper;
using Ecommerce.Application.Handlers.Colors.Commands;
using Ecommerce.Application.Handlers.Colors.Queries;
using Ecommerce.Application.Handlers.Products.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class ColorController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public ColorController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Permissions.Permissions_Color_View)]
    public IActionResult Index() => View();

    [Authorize(Permissions.Permissions_Color_View)]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetColorsWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });
        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    [Authorize(Permissions.Permissions_Color_Create)]
    public IActionResult Create() => View();

    [Authorize(Permissions.Permissions_Color_Create)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateColorCommand command)
    {
        var isColorExists = await _mediator.Send(new IsColorNameOrHexExistQuery { Name = command.Name, HexCode = command.HexCode });
        if (isColorExists) ModelState.AddModelError(string.Empty, "Color Name or Hex Code already exist.");

        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction(nameof(Index));
        }
        return View(command);
    }

    [Authorize(Permissions.Permissions_Color_Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var color = await _mediator.Send(new GetColorByIdQuery { Id = id });
        if (color == null)
        {
            return NotFound();
        }
        var colorCommand = _mapper.Map<UpdateColorCommand>(color);
        return View(colorCommand);
    }

    [Authorize(Permissions.Permissions_Color_Edit)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateColorCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        return View(command);
    }

    [Authorize(Permissions.Permissions_Color_Delete)]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteColorCommand { Id = id });
        return Json(response);
    }
}
