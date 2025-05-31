using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, IUserService userService, IEmailService emailService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _userService = userService;
        _emailService = emailService;
        _logger = logger;
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid) return View(registerUserDto);
        var rs = await _accountService.RegisterUserAsync(registerUserDto);
        if (rs.Succeeded)
            return RedirectToAction("Login", new { succeeded = rs.Succeeded, message = rs.Message });
        ModelState.AddModelError(string.Empty, rs.Message);
        return View(registerUserDto);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginUserDto());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto, string returnUrl)
    {


        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login attempt failed due to invalid model state.");
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning($"Validation error in {state.Key}: {error.ErrorMessage}");
                }
            }
            return View(loginUserDto);
        }

        var rs = await _accountService.SignInAsync(loginUserDto);
        if (rs.Succeeded)
        {
            _logger.LogInformation("User {UserName} logged in successfully.", loginUserDto.UserName);

            TempData["notification"] = "<script>swal(`" + "Welcome Back!" + "`, `" + "Hello " + loginUserDto.UserName + ", Welcome back!" + "`,`" + "success" + "`)" + "</script>";
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        else if (rs.Data.RequiresTwoFactor)
        {
            var sendCode = true;
            return RedirectToAction("LoginTwoStep", new { loginUserDto.UserName, loginUserDto.RememberMe, returnUrl, sendCode });
        }
        else if (rs.Data.IsLockedOut)
        {

            ModelState.AddModelError(string.Empty, "Account locked");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
        }

        return View(loginUserDto);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginTwoStep(string username, bool rememberMe, string returnUrl, bool sendCode = false)
    {
        TwoStepDto twoStepDto = new TwoStepDto { RememberMe = rememberMe, ReturnUrl = returnUrl, UserName = username };
        if (sendCode == true)
        {
            try
            {
                await _emailService.SendTwoFactorEmailAsync(username, "Two-Factor Code");
            }
            catch
            {
                TempData["notification"] = "<script>swal(`" + "Error Occurred!" + "`, `" + "Can't send Two-factor code. please contact support." + "`,`" + "error" + "`)" + "</script>";
                return Redirect("/");
            }
        }
        return View(twoStepDto);
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginTwoStep(TwoStepDto twoStep)
    {
        if (!ModelState.IsValid)
        {
            return View(twoStep);
        }

        var rs = await _accountService.TwoFactorSignInAsync(twoStep);
        if (rs.Succeeded)
        {
            TempData["notification"] = "<script>swal(`" + "Welcome Back!" + "`, `" + "Hello " + twoStep.UserName + ", Welcome back!" + "`,`" + "success" + "`)" + "</script>";
            if (Url.IsLocalUrl(twoStep.ReturnUrl))
            {
                return Redirect(twoStep.ReturnUrl);
            }
            return Redirect("/");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid Code! Please try again.");
            return View(twoStep);
        }
    }


    [HttpGet]
    [Route("logout")]
    [Route("Account/Logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await _accountService.SignOutAsync();
        return Redirect("/");
    }

    [Route("accessdenied")]
    [Route("Account/AccessDenied")]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        try
        {
            var returnUrl = Request.Headers["Referer"].ToString();
            var msg = "<script>swal(`" + "Access Denied!" + "`, `" + "You are not allowed to view this resource." + "`,`" + "warning" + "`)" + "</script>";
            TempData["notification"] = msg;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("/");
        }
        catch
        {
            return View();
        }
    }

    [HttpGet]
    [Route("forgotpassword")]
    [Route("Account/ForgotPassword")]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        if (!ModelState.IsValid) return View(forgotPasswordDto);
        var rs = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);
        var baseUrl = Request.Host.Value;
        if (rs.Succeeded)
        {
            if (rs.Data.EmailConfirmed == false)
            {
                ModelState.AddModelError(string.Empty, "Sorry! Email is Not Confirmed!");
                return View(forgotPasswordDto);
            }
            try
            {
                var url = $"{Request.Scheme}://{Request.Host.Value}/ResetPassword";
                await _emailService.SendResetPasswordEmailAsync(forgotPasswordDto.Email, "Reset Password", url);
                TempData["notification"] = "<script>swal(`" + "Success!" + "`, `" + "A Password Reset link send to your email." + "`,`" + "success" + "`)" + "</script>";
                return Redirect("/");
            }
            catch
            {
                TempData["notification"] = "<script>swal(`" + "Error Occurred!" + "`, `" + "Can't send Two-factor code. please contact support." + "`,`" + "error" + "`)" + "</script>";
                return Redirect("/my/login");
            }
        }
        ModelState.AddModelError(string.Empty, "No User found with this email!");
        return View(forgotPasswordDto);
    }

    [HttpGet]
    [Route("resetpassword")]
    [Route("Account/ResetPassword")]
    [AllowAnonymous]
    public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        return View(resetPasswordDto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPasswordConfirmation(ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(ResetPassword), resetPasswordDto);

        var rs = await _accountService.ResetPasswordAsync(resetPasswordDto);
        if (rs.Succeeded)
        {
            TempData["notification"] = "<script>swal(`" + "Success!" + "`, `" + "Your password is changed successfully. Please login with new password to continue." + "`,`" + "success" + "`)" + "</script>";
            return Redirect("/my/login");
        }

        return RedirectToAction(nameof(ResetPassword), resetPasswordDto);
    }

    [HttpGet]
    [Route("confirmemail")]
    [Route("Account/ConfirmEmail")]
    [AllowAnonymous]
    public IActionResult ConfirmEmail(string returnUrl)
    {
        EmailConfirmationDto emailConfirmationDto = new EmailConfirmationDto { ReturnUrl = returnUrl };
        return View(emailConfirmationDto);
    }


    [HttpGet]
    [Route("emailconfirmation")]
    [Route("Account/EmailConfirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> EmailConfirmation(string email, string token)
    {
        var rs = await _accountService.ConfirmEmailAsync(email, token);
        if (rs.Succeeded)
        {
            TempData["notification"] = "<script>swal(`" + "Success!" + "`, `" + "Your Email is confirmed successfully. Please login to continue." + "`,`" + "success" + "`)" + "</script>";
        }
        else
        {
            TempData["notification"] = "<script>swal(`" + "Error Occurred!" + "`, `" + "Invalid operation." + "`,`" + "error" + "`)" + "</script>";
        }
        return Redirect("/my/login");
    }

}

