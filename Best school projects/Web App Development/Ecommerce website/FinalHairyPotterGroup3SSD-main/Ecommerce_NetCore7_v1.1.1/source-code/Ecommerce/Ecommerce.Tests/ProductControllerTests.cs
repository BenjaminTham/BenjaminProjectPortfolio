using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Categories.Queries;
using Ecommerce.Application.Handlers.Products.Commands;
using Ecommerce.Application.Handlers.Products.Queries;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Web.Mvc.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Tests
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;
        private readonly Mock<ITempDataDictionary> _tempDataMock;

        public ProductControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _tempDataMock = new Mock<ITempDataDictionary>();

            _controller = new ProductController(_mediatorMock.Object, _loggerMock.Object)
            {
                TempData = _tempDataMock.Object
            };

            // Set up ControllerContext
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        // Helper method to set user claims
        private void SetUserWithClaims(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        // Test for GET request to Create action with authorized user, should able to retrieve a list of category
        [Fact]
        public async Task Create_Get_AuthorizeUser_ReturnsViewWithCategorySelectList()
        {
            // Arrange
            SetUserWithClaims("Permissions_Product_View");
            var categories = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Category1" },
                new CategoryDto { Id = 2, Name = "Category2" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(categories);

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var selectList = Assert.IsType<SelectList>(viewResult.ViewData["CategoryId"]);
            Assert.Equal(2, selectList.Count());
        }

        // Test for POST request to Create action with valid model and authorized user
        [Fact]
        public async Task Create_Post_ValidModel_AuthorizeUser_RedirectsToIndex()
        {
            // Arrange
            SetUserWithClaims("Permissions_Product_Create"); //allow user to create
            var createProductCommand = new CreateProductCommand //create the product
            {
                Name = "TestProduct",
                Slug = "test-product",
                CategoryId = 1
            };

            var response = Response<string>.Success("Product created successfully"); //return the response
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(response);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<CategoryDto>());

            // Act
            var result = await _controller.Create(createProductCommand); //test the setup

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); //should redirect if successful creation
            Assert.Equal("Index", redirectResult.ActionName);
        }

        // Test for POST request to Create action with invalid model and authorized user
        [Fact]
        public async Task Create_Post_InvalidModel_AuthorizeUser_ReturnsViewWithModelError()
        {
            // Arrange
            SetUserWithClaims("Permissions_Product_Create");
            var createProductCommand = new CreateProductCommand
            {
                // Name is left null to simulate invalid model
                Slug = "test-product",
                CategoryId = 1
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<CategoryDto>
                         {
                             new CategoryDto { Id = 1, Name = "Category1" }
                         });

            _mediatorMock.Setup(m => m.Send(It.IsAny<IsProductSlugExistQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);

            // Simulate model validation by adding an error to the ModelState
            _controller.ModelState.AddModelError("Name", "Product name is required.");

            // Act
            var result = await _controller.Create(createProductCommand);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(createProductCommand, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("Name"));
            Assert.Contains(_controller.ModelState["Name"].Errors, e => e.ErrorMessage == "Product name is required.");
        }

        // Test for POST request to Create action when product slug already exists
        [Fact]
        public async Task Create_Post_SlugExists_AuthorizeUser_ReturnsViewWithModelError()
        {
            // Arrange
            SetUserWithClaims("Permissions_Product_Create");
            var createProductCommand = new CreateProductCommand
            {
                Name = "TestProduct",
                Slug = "test-product",
                CategoryId = 1
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<CategoryDto>());
            _mediatorMock.Setup(m => m.Send(It.IsAny<IsProductSlugExistQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(createProductCommand);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(createProductCommand, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
        }

        // Test for GET request to Create action with unauthorized user, should return AccessDenied view
        [Fact]
        public async Task Create_Get_UnauthorizedUser_ReturnsAccessDeniedView()
        {
            // Arrange
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            _controller.TempData = tempData;

            // Inject mock authorization service
            _controller.ControllerContext.HttpContext.RequestServices = new ServiceCollection()
                .AddSingleton(mockAuthorizationService.Object)
                .BuildServiceProvider();

            // Act
            var result = await _controller.Create();

            // Check that the ViewData does not contain the CategoryId key or it is empty
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("CategoryId"), "ViewData should not contain CategoryId for unauthorized user.");
        }




    }
}
