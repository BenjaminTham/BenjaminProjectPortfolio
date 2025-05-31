using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.CustomerAccount.Commands;
using Ecommerce.Application.Handlers.Customers.Commands;
using Ecommerce.Application.Handlers.Customers.Queries;
using Ecommerce.Application.Handlers.NewFolder.Queries;
using Ecommerce.Application.Handlers.Orders.Queries;
using Ecommerce.Application.Handlers.OrderStatuses.Queries;
using Ecommerce.Application.Handlers.ProductReviews.Commands;
using Ecommerce.Application.Handlers.RenderItems.Queries;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Web.Mvc.Extension;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class MyController : Controller
{
    #region Basic Config
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    private readonly IAccountService _accountService;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IProfileService _profileService;
    private readonly IEmailService _emailService;
    private readonly ILogger<MyController> _logger;

    public MyController(ILogger<MyController> logger, IMediator mediator,
        IMapper mapper,
        IKeyAccessor keyAccessor,
        IAccountService accountService,
        IProfileService profileService,
        IUserService userService,
        ICurrentUser currentUser,
        IEmailService emailService)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
        _accountService = accountService;
        _profileService = profileService;
        _userService = userService;
        _currentUser = currentUser;
        _emailService = emailService;
    }

    public IHttpContextAccessor? HttpContextAccessor { get; set; }

    #endregion

    public async Task<IActionResult> Index()
    {
        var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());
        var currentUser = await _userService.GetCurrentUsersAsync();
        ViewBag.CustomerInfo = customerInfo;
        ViewBag.CurrentUser = currentUser;
        return View();
    }

    #region Order
    public async Task<IActionResult> Orders(int id)
    {
        var getOrders = await _mediator.Send(new GetCurrentCustomerOrdersQuery());
        return View(getOrders);
    }
    #endregion

    #region Reviews
    public async Task<IActionResult> Reviews()
    {
        var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());
        var customerReviews = new List<ProductReviewDto>();
        if (customerInfo != null)
        {
            customerReviews = await _mediator.Send(new GetCustomerReviewsByCustomerId { CustomerId = customerInfo.Id });

        };
        ViewBag.CustomerReviews = customerReviews;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> CreateReview(CreateProductReviewByCustomerCommand data)
    {

        // Updated regex to include numerals and common punctuation
        Regex regex = new Regex(@"^[a-zA-Z0-9\s.,!?-]*$");
        if (!regex.IsMatch(data.Comment))
        {
            _logger.LogWarning("Invalid comment format. Only alphabets, numbers, spaces, and common punctuation are allowed.");
            return new JsonResult(new JsonResponse { Success = false, Error = "Invalid comment format. Only alphabets, numbers, spaces, and common punctuation are allowed." });
        }

        var response = await _mediator.Send(data);
        if (response.Succeeded)
        {
            _logger.LogInformation("Review created successfully.");
            return new JsonResult(new JsonResponse { Success = true, Data = response });
        }

        _logger.LogWarning("Review creation failed. Response unsuccessful.");
        return Json(new JsonResponse { Success = false, Error = "Failed to process review." });
    }



    #endregion

    #region Wishlist
    public IActionResult WishList() => View();

    [HttpGet]
    public async Task<IActionResult> GetWishListItems(string ids)
    {
        if (ids.IsNullOrEmpty()) return Json(String.Empty);
        long[] productIds = JsonSerializer.Deserialize<long[]>(ids)!;
        var wishListItems = await _mediator.Send(new GetWishListItemByProductIdsQuery { ProductIds = productIds });
        return Json(wishListItems);
    }
    #endregion

    #region Address
    [HttpGet]
    public async Task<IActionResult> Address()
    {
        var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());
        return View(customerInfo);
    }

    [HttpPost]
    public async Task<IActionResult> Address(CustomerDto customer)
    {
        var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());

        if (!ModelState.IsValid)
        {
            ModelState.Remove("UserFirstName");
            ModelState.Remove("UserLastName");

            if (ModelState.IsValid)  // Recheck the ModelState after removing specific errors
            {

                var response = await _mediator.Send(new UpdateCustomerAddressCommand { Customer = customer });
                if (response.Succeeded)
                {
                    return View(customer);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(customer);
                }
            }
            else
            {

                // Log each model state error
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogError($"Error in {entry.Key}: {error.ErrorMessage}");
                    }
                }
                TempData["AddressError"] = "Address must only contain alphanumeric characters, spaces, #, and common punctuation.";
                return View(customerInfo);
            }
        }

        TempData["AddressError"] = "Address must only contain alphanumeric characters, spaces, #, and common punctuation.";
        return View(customerInfo);

    }


    #endregion

    #region Tracking Order
    [HttpGet]
    public async Task<IActionResult> TrackOrder(string invoiceNo)
    {
        if (invoiceNo == null) { return RedirectToAction("Orders"); }
        var customer = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());
        if (customer == null) { return RedirectToAction("Orders"); }
        var order = await _mediator.Send(new GetOrderByInvoiceNoAndCustomerIdQuery { InvoiceNo = invoiceNo, CustomerId = customer.Id });
        if (order == null) { return RedirectToAction("Orders"); }

        var orderStatus = await _mediator.Send(new GetOrderStatusByIdQuery { OrderId = order.Id });
        ViewBag.CurrentOrderStatus = orderStatus.OrderByDescending(o => o.Id).Select(o => o.OrderStatusValue.StatusValue).FirstOrDefault();
        ViewBag.InvoiceNo = invoiceNo;
        ViewBag.OrderStatus = orderStatus;
        return View();
    }

    #endregion

    #region Account Info
    public async Task<IActionResult> AccountInfo()
    {
        var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());
        return View(customerInfo);
    }

    [HttpPost]
    public async Task<IActionResult> AccountInfo(CustomerDto customer)
    {
        if (!ModelState.IsValid)
        {

            // Log each model state error
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    _logger.LogError($"Error in {entry.Key}: {error.ErrorMessage}");
                }
            }

            var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery());
            TempData["AccountInfoError"] = "Name must contain only letters.";
            return View(customerInfo);
        }
        else
        {
            var response = await _mediator.Send(new UpdateCustomerAccountInfoCommand { Customer = customer });
            if (response.Succeeded) return View(customer);
            else ModelState.AddModelError(string.Empty, response.Message);
            return View(customer);
        }
    }


    #endregion

    #region Customer Login
    [HttpGet]
    [Route("customerlogin")]
    [Route("My/Login")]
    [AllowAnonymous]
    public ActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginUserDto());
    }

    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto, string? returnUrl)
    {
        // Deserialize the advanced configuration if available, otherwise use default
        var conAdv = _keyAccessor?["AdvancedConfiguration"] != null
            ? JsonSerializer.Deserialize<AdvancedConfigurationDto>(_keyAccessor["AdvancedConfiguration"])
            : new AdvancedConfigurationDto();

        // Check if the model state is valid
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login attempt failed due to invalid model state for user: {UserName}", loginUserDto.UserName);
            return View(loginUserDto);
        }

        // Attempt to sign in the user
        var rs = await _accountService.SignInAsync(loginUserDto);
        if (rs.Succeeded)
        {
            // Get user details
            var user = await _userService.GetUserByUserNameAsync(loginUserDto.UserName);

            // Display welcome notification
            TempData["notification"] = $"<script>swal('Welcome Back!', 'Hello {user?.Data?.FullName ?? loginUserDto.UserName}, Welcome back!', 'success')</script>";

            // Redirect user to the return URL if it's local
            if (Url.IsLocalUrl(returnUrl))
            {
                _logger.LogInformation("User {UserName} logged in successfully and redirected to {ReturnUrl}", loginUserDto.UserName, returnUrl);
                return Redirect(returnUrl);
            }

            // Redirect user to the home page if no return URL is provided
            _logger.LogInformation("User {UserName} logged in successfully and redirected to Home", loginUserDto.UserName);
            return RedirectToAction("Index", "Home");
        }
        else if (rs.Data.RequiresTwoFactor)
        {
            // Redirect to two-factor authentication if required
            var sendCode = true;
            _logger.LogInformation("User {UserName} requires two-factor authentication", loginUserDto.UserName);
            return RedirectToAction("LoginTwoStep", "Account", new { loginUserDto.UserName, loginUserDto.RememberMe, returnUrl, sendCode });
        }
        else if (rs.Data.IsLockedOut)
        {
            // Handle account lockout
            _logger.LogWarning("User {UserName} account is locked out", loginUserDto.UserName);
            ModelState.AddModelError(string.Empty, "Account locked");
        }
        else if (conAdv.EnableEmailConfirmation && !rs.Data.IsEmailConfirmed)
        {
            // Handle unconfirmed email
            _logger.LogWarning("User {UserName} email is not confirmed", loginUserDto.UserName);
            ModelState.AddModelError(string.Empty, "Email is Not Confirmed!");
        }
        else
        {
            // Handle invalid login attempt
            _logger.LogWarning("Invalid login attempt for user {UserName}", loginUserDto.UserName);
            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
        }

        // Return the view with the login DTO
        return View(loginUserDto);
    }


    #endregion
    
    #region Customer Register
    [AllowAnonymous]
    public ActionResult Register() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(CustomerRegisterDto customerRegister)
    {
        if (!ModelState.IsValid) return View(customerRegister);
        SecurityConfigurationDto conSec = _keyAccessor?["SecurityConfiguration"] != null ? JsonSerializer.Deserialize<SecurityConfigurationDto>(_keyAccessor["SecurityConfiguration"])! : new SecurityConfigurationDto();
        AdvancedConfigurationDto conAdv = _keyAccessor?["AdvancedConfiguration"] != null ? JsonSerializer.Deserialize<AdvancedConfigurationDto>(_keyAccessor["AdvancedConfiguration"])! : new AdvancedConfigurationDto();

        if (customerRegister?.Password.Length < conSec?.PasswordRequiredLength)
        {
            ModelState.AddModelError(customerRegister.Password, $"The Password must be at least {conSec?.PasswordRequiredLength} characters long.");
        }

        if (customerRegister.Password != customerRegister.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
        }

        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new RegisterCustomerCommand { CustomerRegister = customerRegister });
            if (response.Succeeded)
            {
                if (conAdv.EnableEmailConfirmation)
                {
                    try
                    {
                        var url = $"{Request.Scheme}://{Request.Host.Value}/emailconfirmation";
                        var url2 = $"emailconfirmation";
                        await _emailService.SendEmailConfirmationAsync(customerRegister.UserName, "Email Confirmation", url);
                        TempData["notification"] = $"<script>swal('Success!', 'A confirmation email send to your email.', 'success')</script>";
                        return Redirect("/my/login");
                    }
                    catch
                    {
                        TempData["notification"] = "<script>swal(`" + "Error Occurred!" + "`, `" + "Can't send email confirmation. please contact support." + "`,`" + "error" + "`)" + "</script>";
                        return Redirect("/my/login");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "My", new { succeeded = response.Succeeded, message = response.Message });
                }

            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(customerRegister);
    }
    #endregion

    #region Update Password
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
            }
            ModelState.AddModelError("", response.Message);
        }

        return View(editPassword);
    }
    #endregion

    /*

    #region Customer Login
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto, string? returnUrl)
    {
        // Deserialize the advanced configuration if available, otherwise use default
        var conAdv = _keyAccessor?["AdvancedConfiguration"] != null
            ? JsonSerializer.Deserialize<AdvancedConfigurationDto>(_keyAccessor["AdvancedConfiguration"])
            : new AdvancedConfigurationDto();

        // Check if the model state is valid
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login attempt failed due to invalid model state for user: {UserName}", loginUserDto.UserName);
            return View(loginUserDto);
        }

        // Attempt to sign in the user
        var rs = await _accountService.SignInAsync(loginUserDto);
        if (rs.Succeeded)
        {
            // Get user details
            var user = await _userService.GetUserByUserNameAsync(loginUserDto.UserName);

            // Display welcome notification
            TempData["notification"] = $"<script>swal('Welcome Back!', 'Hello {user?.Data?.FullName ?? loginUserDto.UserName}, Welcome back!', 'success')</script>";

            // Redirect user to the return URL if it's local
            if (Url.IsLocalUrl(returnUrl))
            {
                _logger.LogInformation("User {UserName} logged in successfully and redirected to {ReturnUrl}", loginUserDto.UserName, returnUrl);
                return Redirect(returnUrl);
            }

            // Redirect user to the home page if no return URL is provided
            _logger.LogInformation("User {UserName} logged in successfully and redirected to Home", loginUserDto.UserName);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // Handle specific failure reasons
            if (rs.Message == "Email are Not Confirmed!")
            {
                _logger.LogWarning("User {UserName} email is not confirmed", loginUserDto.UserName);
                ModelState.AddModelError(string.Empty, "Email is Not Confirmed!");
            }
            else if (rs.Message == "Account locked")
            {
                _logger.LogWarning("User {UserName} account is locked out", loginUserDto.UserName);
                ModelState.AddModelError(string.Empty, "Account locked");
            }
            else
            {
                _logger.LogWarning("Invalid login attempt for user {UserName}", loginUserDto.UserName);
                ModelState.AddModelError(string.Empty, rs.Message);
            }
        }

        // Return the view with the login DTO
        return View(loginUserDto);
    }


    #endregion



    #region Customer Register
    [AllowAnonymous]
    public ActionResult Register1() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register1(CustomerRegisterDto1 customerRegister)
    {
        //check if model state is valid, security hceck
        if (!ModelState.IsValid) return View(customerRegister);

        SecurityConfigurationDto conSec = _keyAccessor?["SecurityConfiguration"] != null ? JsonSerializer.Deserialize<SecurityConfigurationDto>(_keyAccessor["SecurityConfiguration"])! : new SecurityConfigurationDto();
        AdvancedConfigurationDto conAdv = _keyAccessor?["AdvancedConfiguration"] != null ? JsonSerializer.Deserialize<AdvancedConfigurationDto>(_keyAccessor["AdvancedConfiguration"])! : new AdvancedConfigurationDto();
        
        //check if password length is correct
        if (customerRegister?.Password.Length < conSec?.PasswordRequiredLength)
        {
            ModelState.AddModelError(customerRegister.Password, $"The Password must be at least {conSec?.PasswordRequiredLength} characters long.");
        }

        //if valid register the user.
        if (ModelState.IsValid)
        {
            customerRegister.Url = $"{Request.Scheme}://{Request.Host.Value}/emailconfirmation"; // Set the URL

            var response = await _mediator.Send(new OriginalRegisterCustomerCommand { CustomerRegister = customerRegister });
            if (response.Succeeded)
            {
                if (conAdv.EnableEmailConfirmation)
                {
                    try
                    {
                        TempData["notification"] = $"<script>swal('Success!', 'A confirmation email send to your email.', 'success')</script>";
                        return Redirect("/my/login1");
                    }
                    catch
                    {
                        TempData["notification"] = "<script>swal(`" + "Error Occurred!" + "`, `" + "Can't send email confirmation. please contact support." + "`,`" + "error" + "`)" + "</script>";
                        return Redirect("/my/login1");
                    }
                }
                else
                {
                    return RedirectToAction("Login1", "My", new { succeeded = response.Succeeded, message = response.Message });
                }
            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(customerRegister);
    }


    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !_tokenService.ValidateToken(token, user.Email))
        {
            return BadRequest("Invalid token or email.");
        }

        user.EmailConfirmed = true;
        await _db.SaveChangesAsync();
        return Ok("Email confirmed successfully.");
    }


    #endregion

    */
}
