using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Configuration.Commands;
using Ecommerce.Application.Handlers.Configuration.Queries;
using Ecommerce.Application.Identity;
using Ecommerce.Domain.Identity.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IRoleService _roleService;
    public SettingsController(IMediator mediator, IMapper mapper, IHostApplicationLifetime appLifetime, IRoleService roleService)
    {
        _mediator = mediator;
        _mapper = mapper;
        _appLifetime = appLifetime;
        _roleService = roleService;
    }
    public IActionResult Index()
    {
        return RedirectToAction("Stock");
    }
    [Authorize(Permissions.Permissions_Configuration_Stock)]
    public async Task<IActionResult> Stock()
    {
        var result = await _mediator.Send(new GetStockConfigurationQuery());
        return View(result);
    }
    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Stock)]
    public async Task<IActionResult> Stock(StockConfigurationDto stockConfiguration)
    {
        var updateStockConfiguration = _mapper.Map<UpdateStockConfigurationCommand>(stockConfiguration);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateStockConfiguration);
            if (response.Succeeded) return View(stockConfiguration);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(stockConfiguration);
    }

    [Authorize(Permissions.Permissions_Configuration_Smtp)]
    public async Task<IActionResult> Smtp()
    {
        var result = await _mediator.Send(new GetSmtpConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Smtp)]
    public async Task<IActionResult> Smtp(SmtpConfigurationDto smtpConfigurationDto)
    {
        var updateSmtpConfiguration = _mapper.Map<UpdateSmtpConfigurationCommand>(smtpConfigurationDto);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateSmtpConfiguration);
            if (response.Succeeded) return View(smtpConfigurationDto);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(smtpConfigurationDto);
    }

    [Authorize(Permissions.Permissions_Configuration_Security)]
    public async Task<IActionResult> Security()
    {
        var result = await _mediator.Send(new GetSecurityConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Security)]
    public async Task<IActionResult> Security(SecurityConfigurationDto securityConfigurationDto)
    {
        var updateSmtpConfiguration = _mapper.Map<UpdateSecurityConfigurationCommand>(securityConfigurationDto);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateSmtpConfiguration);
            if (response.Succeeded)
            {
                _appLifetime.StopApplication();
                return View(securityConfigurationDto);
            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(securityConfigurationDto);
    }

    [Authorize(Permissions.Permissions_Configuration_Advanced)]
    public async Task<IActionResult> Advanced()
    {
        ViewData["RoleName"] = new SelectList(await _roleService.GetRolesAsync(), "Name", "Name");
        var result = await _mediator.Send(new GetAdvancedConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Advanced)]
    public async Task<IActionResult> UpdateRole(AdvancedConfigurationDto advancedConfigurationDto)
    {
        ViewData["RoleName"] = new SelectList(await _roleService.GetRolesAsync(), "Name", "Name", advancedConfigurationDto.RoleName);
        var updateAdvancedConfiguration = _mapper.Map<UpdateAdvancedConfigurationCommand>(advancedConfigurationDto);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateAdvancedConfiguration);
            if (response.Succeeded)
            {
                return RedirectToAction(nameof(Advanced));
            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return RedirectToAction(nameof(Advanced));
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Advanced)]
    public async Task<IActionResult> UpdateWebsiteMode(AdvancedConfigurationDto advancedConfigurationDto)
    {
        ViewData["RoleName"] = new SelectList(await _roleService.GetRolesAsync(), "Name", "Name", advancedConfigurationDto.RoleName);
        var updateAdvancedConfiguration = _mapper.Map<UpdateAdvancedConfigurationCommand>(advancedConfigurationDto);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateAdvancedConfiguration);
            if (response.Succeeded)
            {
                return RedirectToAction(nameof(Advanced));
            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return RedirectToAction(nameof(Advanced));
    }



    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Advanced)]
    public async Task<IActionResult> Advanced(AdvancedConfigurationDto advancedConfigurationDto)
    {
        ViewData["RoleName"] = new SelectList(await _roleService.GetRolesAsync(), "Name", "Name", advancedConfigurationDto.RoleName);
        var updateAdvancedConfiguration = _mapper.Map<UpdateAdvancedConfigurationCommand>(advancedConfigurationDto);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateAdvancedConfiguration);
            if (response.Succeeded)
            {
                _appLifetime.StopApplication();
                return View(advancedConfigurationDto);
            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(advancedConfigurationDto);
    }

}
