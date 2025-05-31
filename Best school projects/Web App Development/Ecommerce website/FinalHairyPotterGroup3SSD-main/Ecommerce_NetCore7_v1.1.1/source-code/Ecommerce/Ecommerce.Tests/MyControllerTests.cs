using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.CustomerAccount.Commands;
using Ecommerce.Application.Interfaces;
using Ecommerce.Web.Mvc.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Domain.Common;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Ecommerce.Application.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Ecommerce.Tests
{
    public class MyControllerTests
    {
        private readonly MyController _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IKeyAccessor> _keyAccessorMock;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IProfileService> _profileServiceMock;
        private readonly Mock<ILogger<MyController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;

        public MyControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _emailServiceMock = new Mock<IEmailService>();
            _keyAccessorMock = new Mock<IKeyAccessor>();
            _accountServiceMock = new Mock<IAccountService>();
            _userServiceMock = new Mock<IUserService>();
            _profileServiceMock = new Mock<IProfileService>();
            _loggerMock = new Mock<ILogger<MyController>>();
            _mapperMock = new Mock<IMapper>();
            _urlHelperMock = new Mock<IUrlHelper>();

            _controller = new MyController(
                _loggerMock.Object,
                _mediatorMock.Object,
                _mapperMock.Object,
                _keyAccessorMock.Object,
                _accountServiceMock.Object,
                _profileServiceMock.Object,
                _userServiceMock.Object,
                null,
                _emailServiceMock.Object)
            {
                HttpContextAccessor = new Mock<IHttpContextAccessor>().Object
            };

            // Initialize TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            // Initialize UrlHelper
            _controller.Url = _urlHelperMock.Object;
        }


        [Fact]
        public async Task Register_ValidModel_RedirectsToLogin()
        {
            // Arrange: Setting up the necessary objects and mock behaviors
            var customerRegisterDto = new CustomerRegisterDto
            {
                UserName = "testuser", // Setting the username for the test user
                FirstName = "Test", // Setting the first name for the test user
                LastName = "User", // Setting the last name for the test user
                Email = "testuser@example.com", // Setting the email for the test user
                Password = "Password123!", // Setting the password for the test user
                ConfirmPassword = "Password123!" // Setting the confirm password to match the password
            };

            // Creating a successful response for the registration
            var response = Response<UserIdentityDto>.Success(new UserIdentityDto { Id = "testuserId" }, "Registration succeeded");

            // Setting up the mediator mock to return the successful response when a RegisterCustomerCommand is sent
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCustomerCommand>(), default(CancellationToken))).ReturnsAsync(response);

            // Act: Calling the Register method with the customerRegisterDto
            var result = await _controller.Register(customerRegisterDto);

            // Assert: Verifying the result of the action
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); // Asserting that the result is a RedirectToActionResult
            Assert.Equal("Login", redirectResult.ActionName); // Asserting that the action to which we are redirected is "Login"
            Assert.Equal("My", redirectResult.ControllerName); // Asserting that the controller to which we are redirected is "My"
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange: Setting up an invalid model state
            _controller.ModelState.AddModelError("Error", "Invalid model");

            var customerRegisterDto = new CustomerRegisterDto();

            // Act: Calling the Register method with the invalid customerRegisterDto
            var result = await _controller.Register(customerRegisterDto);

            // Assert: Verifying the result of the action
            var viewResult = Assert.IsType<ViewResult>(result); // Asserting that the result is a ViewResult
            Assert.Equal(customerRegisterDto, viewResult.Model); // Asserting that the model passed to the view is the same as the one provided
        }

        [Fact]
        public async Task Register_PasswordMismatch_ReturnsViewWithModelError()
        {
            // Arrange
            var customerRegisterDto = new CustomerRegisterDto
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password456!" // Mismatched password
            };

            var conSec = new SecurityConfigurationDto { PasswordRequiredLength = 8 };
            var conAdv = new AdvancedConfigurationDto { EnableEmailConfirmation = true };

            _keyAccessorMock.Setup(k => k["SecurityConfiguration"]).Returns(JsonSerializer.Serialize(conSec));
            _keyAccessorMock.Setup(k => k["AdvancedConfiguration"]).Returns(JsonSerializer.Serialize(conAdv));

            // Act
            var result = await _controller.Register(customerRegisterDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(customerRegisterDto, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("ConfirmPassword"));
            Assert.Contains(_controller.ModelState["ConfirmPassword"].Errors, e => e.ErrorMessage == "The password and confirmation password do not match.");
        }


        [Fact]
        public async Task Register_ValidModelWithEmailConfirmation_SendsEmailConfirmation()
        {
            // Arrange
            var customerRegisterDto = new CustomerRegisterDto
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = Response<UserIdentityDto>.Success(new UserIdentityDto { Id = "testuserId" }, "Registration succeeded");

            var conAdv = new AdvancedConfigurationDto { EnableEmailConfirmation = true };
            _keyAccessorMock.Setup(k => k["AdvancedConfiguration"]).Returns(JsonSerializer.Serialize(conAdv));

            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCustomerCommand>(), default(CancellationToken))).ReturnsAsync(response);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost");
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Register(customerRegisterDto);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailConfirmationAsync(
                customerRegisterDto.UserName,
                "Email Confirmation",
                It.IsAny<string>()), Times.Once);

            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/my/login", redirectResult.Url);
        }



        // New Test for password length validation
        [Fact]
        public async Task Register_PasswordTooShort_ReturnsViewWithModelError()
        {
            // Arrange
            var customerRegisterDto = new CustomerRegisterDto
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Short1!",
                ConfirmPassword = "Short1!"
            };

            var conSec = new SecurityConfigurationDto { PasswordRequiredLength = 8 };
            _keyAccessorMock.Setup(k => k["SecurityConfiguration"]).Returns(JsonSerializer.Serialize(conSec));

            // Act
            var result = await _controller.Register(customerRegisterDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(customerRegisterDto, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("Short1!"));
            Assert.Contains(_controller.ModelState["Short1!"].Errors, e => e.ErrorMessage == "The Password must be at least 8 characters long.");
        }

        // New Test for registration failure due to existing username
        [Fact]
        public async Task Register_UsernameExists_ReturnsViewWithModelError()
        {
            // Arrange
            var customerRegisterDto = new CustomerRegisterDto
            {
                UserName = "existinguser",
                FirstName = "Test",
                LastName = "User",
                Email = "existinguser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = Response<UserIdentityDto>.Fail("Username already exists");

            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCustomerCommand>(), default(CancellationToken))).ReturnsAsync(response);

            // Act
            var result = await _controller.Register(customerRegisterDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(customerRegisterDto, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains(_controller.ModelState[string.Empty].Errors, e => e.ErrorMessage == "Username already exists");
        }


        [Fact]
        public async Task Login_ValidModel_RedirectsToHomeIndex()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                UserName = "testuser",
                Password = "Password123!",
                RememberMe = false
            };

            var response = Response<UserIdentityDto>.Success(new UserIdentityDto { Id = "testuser" }, "Login succeeded");
            _accountServiceMock.Setup(a => a.SignInAsync(loginUserDto)).ReturnsAsync(response);

            var userResponse = Response<AddEditUserDto>.Success(new AddEditUserDto { UserName = "testuser", FullName = "Test User" }, "Success");
            _userServiceMock.Setup(u => u.GetUserByUserNameAsync(loginUserDto.UserName)).ReturnsAsync(userResponse);

            // Act
            var result = await _controller.Login(loginUserDto, null); // Use null for returnUrl

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }



        [Fact]
        public async Task Login_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model");

            var loginUserDto = new LoginUserDto();

            // Act
            var result = await _controller.Login(loginUserDto, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginUserDto, viewResult.Model);
        }

        [Fact]
        public async Task Login_InvalidLogin_ReturnsViewWithModelError()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                UserName = "invaliduser",
                Password = "WrongPassword!",
                RememberMe = false
            };

            var response = Response<UserIdentityDto>.Fail(new UserIdentityDto { RequiresTwoFactor = false, IsLockedOut = false, IsEmailConfirmed = false }, "Invalid login attempt");
            _accountServiceMock.Setup(a => a.SignInAsync(loginUserDto)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(loginUserDto, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginUserDto, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains(_controller.ModelState[string.Empty].Errors, e => e.ErrorMessage == "Invalid Login Attempt");
        }

        [Fact]
        public async Task Login_EmailNotConfirmed_ReturnsViewWithModelError()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                UserName = "testuser",
                Password = "Password123!",
                RememberMe = false
            };

            var response = Response<UserIdentityDto>.Fail(new UserIdentityDto { RequiresTwoFactor = false, IsLockedOut = false, IsEmailConfirmed = false }, "Email not confirmed");
            _accountServiceMock.Setup(a => a.SignInAsync(loginUserDto)).ReturnsAsync(response);

            var advancedConfig = new AdvancedConfigurationDto { EnableEmailConfirmation = true };
            _keyAccessorMock.Setup(k => k["AdvancedConfiguration"]).Returns(JsonSerializer.Serialize(advancedConfig));

            // Act
            var result = await _controller.Login(loginUserDto, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginUserDto, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Contains(_controller.ModelState[string.Empty].Errors, e => e.ErrorMessage == "Email is Not Confirmed!");
        }


        [Fact]
        public async Task Password_ValidModel_ChangesPasswordAndRedirects()
        {
            // Arrange
            var editPasswordDto = new EditPasswordDto
            {
                OldPassword = "CurrentPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            // Correct the type expected by the mock setup
            var response = Response<UserIdentityDto>.Success(new UserIdentityDto { Id = "testuser" }, "Password changed successfully");
            _profileServiceMock.Setup(p => p.UpdatePasswordAsync(It.IsAny<EditPasswordDto>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Password(editPasswordDto);

            // Assert
            _accountServiceMock.Verify(a => a.SignOutAsync(), Times.Once);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/my/login", redirectResult.Url);
        }

        [Fact]
        public async Task Password_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model");

            var editPasswordDto = new EditPasswordDto
            {
                OldPassword = "CurrentPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };

            // Act
            var result = await _controller.Password(editPasswordDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editPasswordDto, viewResult.Model);
        }


        [Fact]
        public async Task Password_PasswordMismatch_ReturnsViewWithModelError()
        {
            // Arrange
            var editPasswordDto = new EditPasswordDto
            {
                OldPassword = "CurrentPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "MismatchPassword123!" // Mismatched confirm password
            };

            // Explicitly adding the model error for password mismatch
            _controller.ModelState.AddModelError("ConfirmNewPassword", "The password and confirmation password do not match.");

            // Act
            var result = await _controller.Password(editPasswordDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editPasswordDto, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("ConfirmNewPassword"));
        }




    }
}
