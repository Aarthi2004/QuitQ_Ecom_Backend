using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace QuitQ_Ecom_Test
{
    public class UserAddressControllerTest
    {
        private UserAddressController _userAddressController;
        private Mock<IUserAddressService> _userAddressServiceMock;
        private Mock<ILogger<UserAddressController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userAddressServiceMock = new Mock<IUserAddressService>();
            _loggerMock = new Mock<ILogger<UserAddressController>>();
            _userAddressController = new UserAddressController(_userAddressServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task AddUserAddress_ReturnsCreatedAtAction()
        {
            // Arrange
            var userAddressDTO = new UserAddressDTO
            {
                UserId = 1,
                DoorNumber = "123",
                Street = "Main Street",
                CityId = 1,
                PostalCode = "12345",
                ContactNumber = "1234567890"
            };
            _userAddressServiceMock.Setup(service => service.AddUserAddress(userAddressDTO)).ReturnsAsync(userAddressDTO);

            // Act
            var result = await _userAddressController.AddUserAddress(userAddressDTO);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(nameof(UserAddressController.AddUserAddress), createdAtActionResult.ActionName);
        }

        [Test]
        public async Task UpdateUserAddress_ReturnsOk()
        {
            // Arrange
            int userAddressId = 1;
            var userAddressToUpdate = new UserAddressDTO
            {
                UserAddressId = userAddressId,
                UserId = 1,
                DoorNumber = "456",
                Street = "Updated Street",
                CityId = 1,
                PostalCode = "54321",
                ContactNumber = "9876543210"
            };
            _userAddressServiceMock.Setup(service => service.UpdateUserAddress(userAddressId, userAddressToUpdate)).ReturnsAsync(userAddressToUpdate);

            // Act
            var result = await _userAddressController.UpdateUserAddress(userAddressId, userAddressToUpdate);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(userAddressToUpdate, okResult.Value);
        }

        [Test]
        public async Task UpdateUserAddress_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int userAddressId = 999;
            var userAddressDTO = new UserAddressDTO();
            _userAddressServiceMock.Setup(service => service.UpdateUserAddress(userAddressId, userAddressDTO))
                .ThrowsAsync(new UserAddressNotFoundException());

            // Act
            var result = await _userAddressController.UpdateUserAddress(userAddressId, userAddressDTO);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User Address not found", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteUserAddress_ReturnsOk()
        {
            // Arrange
            int userAddressId = 1;
            _userAddressServiceMock.Setup(service => service.DeleteUserAddress(userAddressId)).ReturnsAsync(true);

            // Act
            var result = await _userAddressController.DeleteUserAddress(userAddressId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("User Address deleted successfully", okResult.Value);
        }

        [Test]
        public async Task DeleteUserAddress_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int userAddressId = 999;
            _userAddressServiceMock.Setup(service => service.DeleteUserAddress(userAddressId))
                .ThrowsAsync(new UserAddressNotFoundException());

            // Act
            var result = await _userAddressController.DeleteUserAddress(userAddressId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User Address not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetUserAddressesByUserId_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var userAddresses = new List<UserAddressDTO>
            {
                new UserAddressDTO { UserAddressId = 1, UserId = userId, DoorNumber = "123", Street = "Main Street" },
                new UserAddressDTO { UserAddressId = 2, UserId = userId, DoorNumber = "456", Street = "Second Street" }
            };
            _userAddressServiceMock.Setup(service => service.GetUserAddressesByUserId(userId)).ReturnsAsync(userAddresses);

            // Act
            var result = await _userAddressController.GetUserAddressesByUserId(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(userAddresses, okResult.Value);
        }
    }
}