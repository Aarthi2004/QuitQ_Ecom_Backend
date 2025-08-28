using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Service;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QuitQ_Ecom.Exceptions;
using System;

namespace QuitQ_Ecom_Test.Controllers
{
    [TestFixture]
    public class WishlistControllerTests
    {
        private Mock<IWishlistService> _wishlistServiceMock;
        private WishlistController _wishlistController;
        private Mock<ILogger<WishlistController>> _loggerMock;
        private ClaimsPrincipal _mockUser;
        private int _testUserId;

        [SetUp]
        public void Setup()
        {
            _wishlistServiceMock = new Mock<IWishlistService>();
            _loggerMock = new Mock<ILogger<WishlistController>>();
            _wishlistController = new WishlistController(_wishlistServiceMock.Object, _loggerMock.Object);

            // Set up a mock user with a UserId claim
            _testUserId = 1;
            var claims = new List<Claim>
            {
                new Claim("UserId", _testUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _mockUser = new ClaimsPrincipal(identity);

            // Set the Controller's HttpContext to include the mock user
            _wishlistController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _mockUser }
            };
        }

        [Test]
        public async Task GetUserWishList_ReturnsOkWithWishlistItems()
        {
            // Arrange
            var wishlistItems = new List<WishListDTO> { new WishListDTO { WishListId = 1, UserId = _testUserId, ProductId = 1 } };
            _wishlistServiceMock.Setup(service => service.GetUserWishList(_testUserId)).ReturnsAsync(wishlistItems);

            // Act
            var result = await _wishlistController.GetUserWishList();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(wishlistItems, okResult.Value);
        }

        [Test]
        public async Task GetUserWishList_ReturnsOkWithEmptyList_WhenNoItemsFound()
        {
            // Arrange
            _wishlistServiceMock.Setup(service => service.GetUserWishList(_testUserId)).ReturnsAsync(new List<WishListDTO>());

            // Act
            var result = await _wishlistController.GetUserWishList();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var value = okResult.Value as List<WishListDTO>;
            Assert.IsNotNull(value);
            Assert.IsEmpty(value);
        }

        [Test]
        public async Task AddToWishList_ReturnsOkWithAddedItem()
        {
            // Arrange
            var wishlistItemToAdd = new WishListDTO { ProductId = 1 };
            var returnedWishlistItem = new WishListDTO { WishListId = 1, UserId = _testUserId, ProductId = 1 };
            _wishlistServiceMock.Setup(service => service.AddToWishList(It.Is<WishListDTO>(dto => dto.UserId == _testUserId && dto.ProductId == wishlistItemToAdd.ProductId)))
                                .ReturnsAsync(returnedWishlistItem);

            // Act
            var result = await _wishlistController.AddToWishList(wishlistItemToAdd);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(returnedWishlistItem, okResult.Value);
            _wishlistServiceMock.Verify(service => service.AddToWishList(It.Is<WishListDTO>(dto => dto.UserId == _testUserId)), Times.Once);
        }

        [Test]
        public async Task RemoveFromWishList_ReturnsOk_WhenItemIsRemoved()
        {
            // Arrange
            int productIdToRemove = 1;
            _wishlistServiceMock.Setup(service => service.RemoveFromWishList(_testUserId, productIdToRemove)).ReturnsAsync(true);

            // Act
            var result = await _wishlistController.RemoveFromWishList(productIdToRemove);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual("Item removed from wishlist", okResult.Value);
        }

        [Test]
        public async Task RemoveFromWishList_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            int productIdToRemove = 999;
            _wishlistServiceMock.Setup(service => service.RemoveFromWishList(_testUserId, productIdToRemove)).ReturnsAsync(false);

            // Act
            var result = await _wishlistController.RemoveFromWishList(productIdToRemove);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Assert.AreEqual("Item not found in wishlist", notFoundResult.Value);
        }

        [Test]
        public async Task AddToWishList_ThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var wishlistItemToAdd = new WishListDTO { ProductId = 1 };
            _wishlistServiceMock.Setup(service => service.AddToWishList(It.IsAny<WishListDTO>())).ThrowsAsync(new AddToWishlistException("Test exception"));

            // Act
            var result = await _wishlistController.AddToWishList(wishlistItemToAdd);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        // Add more tests for other failure scenarios (e.g., GetUserWishList, RemoveFromWishlist exceptions)
    }
}