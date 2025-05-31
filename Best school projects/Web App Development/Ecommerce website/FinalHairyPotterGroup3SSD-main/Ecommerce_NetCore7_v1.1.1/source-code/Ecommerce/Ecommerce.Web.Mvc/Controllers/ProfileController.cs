using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Areas.Manager.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IAccountService _accountService;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public ProfileController(IProfileService profileService, IAccountService accountService, IMediator mediator, IMapper mapper)
    {
        _profileService = profileService;
        _accountService = accountService;
        _mediator = mediator;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index()
    {
        var response = await _profileService.GetCurrentUserAsync();
        ViewBag.User = response.Data;
        return View(response.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Index(EditProfileDto editProfileDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _profileService.UpdateProfileAsync(editProfileDto);
            if (response.Succeeded) return RedirectToAction("Index");
            ModelState.AddModelError("", response.Message);
        }
        return View("Index", editProfileDto);
    }

    public async Task<IActionResult> Security()
    {
        var rs = await _profileService.GetTwoFactorAuthorizationAsync();
        return View(rs.Data);
    }

    [HttpPost]
    public async Task<IActionResult> TwoFactorAuthorization(TwoStepDto twoStepDto)
    {
        var rs = await _profileService.UpdateTwoFactorAuthorizationAsync(twoStepDto);
        if (rs.Succeeded)
        {
            if (twoStepDto.IsEnabled == true)
            {
                TempData["notification"] = "<script>swal(`" + "Two-Factor Enabled!" + "`, `" + "Please login to continue." + "`,`" + "success" + "`)" + "</script>";
            }
            else
            {
                TempData["notification"] = "<script>swal(`" + "Two-Factor Disabled!" + "`, `" + "Please login to continue." + "`,`" + "success" + "`)" + "</script>";
            }

            //TempData.Keep("notification");
        }
        return Json(rs);
    }

    public IActionResult Password()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Password(EditPasswordDto editPassword)
    {
        if (ModelState.IsValid)
        {
            var response = await _profileService.UpdatePasswordAsync(editPassword);
            if (response.Succeeded)
            {
                await _accountService.SignOutAsync();
                TempData["notification"] = "<script>swal(`" + "Your Password Changed!" + "`, `" + "Please login to continue." + "`,`" + "success" + "`)" + "</script>";
                return Redirect("/my/login");
                //return Redirect("/login");
            }
            ModelState.AddModelError("", response.Message);
        }

        return View(editPassword);
    }

}
