using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Service;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

namespace QuitQ_Ecom.Test
{
    [TestFixture]
    public class CartControllerTests
    {
        private CartController _controller;
        private Mock<ICartService> _cartServiceMock;
        private Mock<ILogger<CartController>> _loggerMock;
        private readonly int _testUserId = 1;

        [SetUp]
        public void Setup()
        {
            _cartServiceMock = new Mock<ICartService>();
            _loggerMock = new Mock<ILogger<CartController>>();

            _controller = new CartController(_cartServiceMock.Object, _loggerMock.Object);

            // Mock the HttpContext and ClaimsPrincipal for authorization
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", _testUserId.ToString()), // Assumes the claim is "UserId"
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };
        }

        [Test]
        public async Task GetUserCartItems_WithValidToken_ReturnsOk()
        {
            // Arrange
            var mockCartItems = new List<CartDTO> { new CartDTO { CartId = 1, UserId = _testUserId, ProductId = 101, Quantity = 2 } };
            _cartServiceMock.Setup(service => service.GetUserCartItems(_testUserId)).ReturnsAsync(mockCartItems);

            // Act
            var result = await _controller.GetUserCartItems();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(mockCartItems, okResult.Value);
        }

        [Test]
        public async Task GetUserCartItems_WithException_Returns500InternalServerError()
        {
            // Arrange
            _cartServiceMock.Setup(service => service.GetUserCartItems(It.IsAny<int>())).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetUserCartItems();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task AddProductToCart_WithValidData_ReturnsOk()
        {
            // Arrange
            var cartItemDto = new CartDTO { ProductId = 101, Quantity = 1 };
            var returnedCartItem = new CartDTO { CartId = 1, UserId = _testUserId, ProductId = 101, Quantity = 1 };
            _cartServiceMock.Setup(service => service.AddProductToCart(cartItemDto, _testUserId)).ReturnsAsync(returnedCartItem);

            // Act
            var result = await _controller.AddProductToCart(cartItemDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(returnedCartItem, okResult.Value);
        }

        [Test]
        public async Task AddProductToCart_WithException_Returns500InternalServerError()
        {
            // Arrange
            var cartItemDto = new CartDTO { ProductId = 101, Quantity = 1 };
            _cartServiceMock.Setup(service => service.AddProductToCart(cartItemDto, _testUserId)).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.AddProductToCart(cartItemDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task IncreaseProductQuantity_WithValidCartItemId_ReturnsOk()
        {
            // Arrange
            int cartItemId = 1;
            _cartServiceMock.Setup(service => service.IncreaseProductQuantity(cartItemId, _testUserId)).ReturnsAsync(true);

            // Act
            var result = await _controller.IncreaseProductQuantity(cartItemId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Product quantity increased successfully", okResult.Value);
        }

        [Test]
        public async Task IncreaseProductQuantity_WithInvalidCartItemId_ReturnsNotFound()
        {
            // Arrange
            int cartItemId = 999;
            _cartServiceMock.Setup(service => service.IncreaseProductQuantity(cartItemId, _testUserId)).ReturnsAsync(false);

            // Act
            var result = await _controller.IncreaseProductQuantity(cartItemId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DecreaseProductQuantity_WithValidCartItemId_ReturnsOk()
        {
            // Arrange
            int cartItemId = 1;
            _cartServiceMock.Setup(service => service.DecreaseProductQuantity(cartItemId, _testUserId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DecreaseProductQuantity(cartItemId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Product quantity decreased successfully", okResult.Value);
        }

        [Test]
        public async Task DecreaseProductQuantity_WithInvalidCartItemId_ReturnsNotFound()
        {
            // Arrange
            int cartItemId = 999;
            _cartServiceMock.Setup(service => service.DecreaseProductQuantity(cartItemId, _testUserId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DecreaseProductQuantity(cartItemId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DeleteProductFromCart_WithValidCartItemId_ReturnsOk()
        {
            // Arrange
            int cartItemId = 1;
            _cartServiceMock.Setup(service => service.RemoveProductFromCart(cartItemId, _testUserId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProductFromCart(cartItemId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Item removed from cart", okResult.Value);
        }

        [Test]
        public async Task DeleteProductFromCart_WithInvalidCartItemId_ReturnsNotFound()
        {
            // Arrange
            int cartItemId = 999;
            _cartServiceMock.Setup(service => service.RemoveProductFromCart(cartItemId, _testUserId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProductFromCart(cartItemId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetCartTotalCost_WithValidToken_ReturnsOk()
        {
            // Arrange
            decimal totalCost = 100.00m; // Mock total cost
            _cartServiceMock.Setup(service => service.GetCartTotalCost(_testUserId)).ReturnsAsync(totalCost);

            // Act
            var result = await _controller.GetCartTotalCost();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(totalCost, okResult.Value);
        }

        [Test]
        public async Task GetCartTotalCost_WithException_Returns500InternalServerError()
        {
            // Arrange
            _cartServiceMock.Setup(service => service.GetCartTotalCost(It.IsAny<int>())).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetCartTotalCost();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }
    }
}