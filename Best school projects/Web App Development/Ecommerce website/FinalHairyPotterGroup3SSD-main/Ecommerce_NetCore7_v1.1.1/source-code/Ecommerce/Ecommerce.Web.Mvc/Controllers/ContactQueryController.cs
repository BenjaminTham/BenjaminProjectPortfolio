using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.ContactQueries.Commands;
using Ecommerce.Application.Handlers.ContactQueries.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class ContactQueryController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<ContactQueryController> _logger; // Add this line

    public ContactQueryController(IMediator mediator, IMapper mapper, ILogger<ContactQueryController> logger) // Modify this line
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger; // Initialize the logger
    }

    [Authorize(Permissions.Permissions_ContactQuery_View)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Permissions.Permissions_Customer_View)]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetContactQueriesWithPagingQuery() { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    [AllowAnonymous]
    public IActionResult Send()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(ContactQueryDto dto)
    {

        foreach (var entry in ModelState)
        {
            _logger.LogDebug($"{entry.Key}: {String.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
        }

        if (!ModelState.IsValid)
        {

            TempData["errorMessage"] = "Only alphanumeric characters, spaces, '.', '!', and '?' are allowed.";
            return View(dto);
        }

        var command = _mapper.Map<CreateContactQueryCommand>(dto);
        var response = await _mediator.Send(command);
        if (response.Succeeded)
        {
            TempData["notification"] = "<script>swal('Success!', 'Your message has been sent successfully!', 'success');</script>";
            return Redirect("/");
        }
        else
        {
            TempData["errorMessage"] = response.Message;
            return View(dto);
        }
    }



}
