using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Customers.Queries;
using Ecommerce.Application.Handlers.DeliveryMethods.Queries;
using Ecommerce.Application.Handlers.Orders.Commands;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Web.Mvc.Controllers;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Tests
{
    public class OrderControllerTests
    {
        private readonly OrderController _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;
        private readonly Mock<IRequestCookieCollection> _cookiesMock;

        public OrderControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _cookiesMock = new Mock<IRequestCookieCollection>();

            _controller = new OrderController(
                _mediatorMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object);

            // Initialize TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            // Setup HttpContext and Request
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContextMock.Object
            };

            _httpContextMock.Setup(ctx => ctx.Request).Returns(_httpRequestMock.Object);
            _httpRequestMock.Setup(req => req.Cookies).Returns(_cookiesMock.Object);
        }

        // Test for GET request to Checkout action, should return view with customer info and delivery methods
        [Fact]
        public async Task Checkout_Get_ReturnsViewWithCustomerInfoAndDeliveryMethods()
        {
            // Arrange
            var customerInfo = new CustomerDto { Id = 1, UserFirstName = "John", UserLastName = "Doe", UserEmail = "john.doe@example.com", UserPhoneNumber = "1234567890" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerInfoByLoginUserQuery>(), default)).ReturnsAsync(customerInfo);

            var deliveryMethods = new List<DeliveryMethodDto>
            {
                new DeliveryMethodDto { Id = 1, IsActive = true, Name = "Standard Shipping" },
                new DeliveryMethodDto { Id = 2, IsActive = true, Name = "Express Shipping" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDeliveryMethodsQuery>(), default)).ReturnsAsync(deliveryMethods);

            var cart = new List<CartDto> { new CartDto { Title = "Product 1", Price = 10.0m, Qty = 1, VariantId = 1 } };
            string serializedCart = JsonSerializer.Serialize(cart);
            _cookiesMock.Setup(c => c["shop-cart"]).Returns(serializedCart);

            // Act
            var result = await _controller.Checkout();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(customerInfo, viewResult.ViewData["CustomerInfo"]);
            Assert.Equal(deliveryMethods, viewResult.ViewData["DeliveryMethod"]);
        }

        // Test for POST request to Checkout action with valid data, should return success JSON response
        [Fact]
        public async Task Checkout_Post_ValidData_ReturnsSuccessJsonResponse()
        {
            // Arrange
            var checkoutDto = new CheckoutDto
            {
                CustomerFirstName = "John",
                CustomerLastName = "Doe",
                CustomerEmail = "john.doe@example.com",
                CustomerPhoneNumber = "1234567890",
                ShippingAddress = "123 Main St, City, Country",
                DeliveryMethod = 1,
                PaymentMethod = "Stripe",
                StripeModel = new StripeModel { Token = "tok_visa" },
                WillSaveInfo = true
            };

            var response = Response<string>.Success("12345"); // Use a numerical string representing order ID
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrderCheckoutCommand>(), default)).ReturnsAsync(response);

            // Act
            var result = await _controller.Checkout(checkoutDto);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonResponse = Assert.IsType<JsonResponse>(jsonResult.Value);
            Assert.True(jsonResponse.Success);
            Assert.Equal("12345", jsonResponse.Data); // Ensure it matches the order ID
        }

        // Test for POST request to Checkout action with invalid data, should return error JSON response
        [Fact]
        public async Task Checkout_Post_InvalidData_ReturnsErrorJsonResponse()
        {
            // Arrange
            var checkoutDto = new CheckoutDto
            {
                CustomerFirstName = "John",
                CustomerLastName = "Doe",
                CustomerEmail = "john.doe@example.com",
                CustomerPhoneNumber = "1234567890",
                ShippingAddress = "123 Main St, City, Country",
                DeliveryMethod = 1,
                PaymentMethod = "Stripe",
                StripeModel = new StripeModel { Token = "tok_visa" },
                WillSaveInfo = true
            };

            _controller.ModelState.AddModelError("CustomerEmail", "Invalid Email Address!");

            // Act
            var result = await _controller.Checkout(checkoutDto);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var jsonResponse = Assert.IsType<JsonResponse>(jsonResult.Value);
            Assert.False(jsonResponse.Success);
            Assert.Equal("Invalid Email Address!", jsonResponse.Message);
        }

        // Test for GET request to Checkout action with empty cart, should redirect to Shop index
        [Fact]
        public async Task Checkout_Get_EmptyCart_ReturnsRedirectToShop()
        {
            // Arrange
            var customerInfo = new CustomerDto { Id = 1, UserFirstName = "John", UserLastName = "Doe", UserEmail = "john.doe@example.com", UserPhoneNumber = "1234567890" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerInfoByLoginUserQuery>(), default)).ReturnsAsync(customerInfo);

            _cookiesMock.Setup(c => c["shop-cart"]).Returns(JsonSerializer.Serialize(new List<CartDto>()));

            // Act
            var result = await _controller.Checkout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Shop", redirectResult.ControllerName);
        }

    }
}
