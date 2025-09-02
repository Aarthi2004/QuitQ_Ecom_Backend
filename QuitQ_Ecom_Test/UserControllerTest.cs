using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class UsersControllerTest
    {
        private UsersController _usersController;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<UsersController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UsersController>>();
            _usersController = new UsersController(_userServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Register_ValidUser_ReturnsOkWithSuccessResponse()
        {
            // Arrange
            var userDTO = new UserDTO
            {
                Username = "testuser",
                Password = "password123",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Dob = new DateTime(2000, 1, 1),
                ContactNumber = "1234567890",
                UserTypeId = 1
            };
            _userServiceMock.Setup(s => s.RegisterUser(It.IsAny<UserDTO>())).ReturnsAsync(userDTO);

            // Act
            var result = await _usersController.Register(userDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            dynamic responseValue = okResult.Value;
            Assert.That((bool)responseValue.success, Is.True);
            Assert.That((UserDTO)responseValue.data, Is.EqualTo(userDTO));
        }

        [Test]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _usersController.ModelState.AddModelError("Username", "The Username field is required.");
            var userDTO = new UserDTO();

            // Act
            var result = await _usersController.Register(userDTO);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            dynamic responseValue = badRequestResult.Value;
            Assert.That((bool)responseValue.success, Is.False);
            Assert.That(responseValue.errors, Is.Not.Null);
        }

        [Test]
        public async Task Register_ServiceThrowsAddUserException_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var userDTO = new UserDTO { Username = "existinguser" };
            var exceptionMessage = "Username already exists.";
            _userServiceMock.Setup(s => s.RegisterUser(userDTO)).ThrowsAsync(new AddUserException(exceptionMessage));

            // Act
            var result = await _usersController.Register(userDTO);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            dynamic responseValue = badRequestResult.Value;
            Assert.That((bool)responseValue.success, Is.False);
            Assert.That((string)responseValue.message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task Register_ServiceThrowsGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var userDTO = new UserDTO { Username = "testuser" };
            _userServiceMock.Setup(s => s.RegisterUser(userDTO)).ThrowsAsync(new Exception("Database error."));

            // Act
            var result = await _usersController.Register(userDTO);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            dynamic responseValue = objectResult.Value;
            Assert.That((bool)responseValue.success, Is.False);
        }

        [Test]
        public async Task GetAllUsers_ReturnsOk()
        {
            // Arrange
            var users = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Username = "user1" },
                new UserDTO { UserId = 2, Username = "user2" }
            };
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _usersController.GetAllUsers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task GetUserByUserType_ReturnsOk()
        {
            // Arrange
            int userTypeId = 1;
            var users = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Username = "user1", UserTypeId = userTypeId },
                new UserDTO { UserId = 2, Username = "user2", UserTypeId = userTypeId }
            };
            _userServiceMock.Setup(s => s.GetUsersByUserType(userTypeId)).ReturnsAsync(users);

            // Act
            var result = await _usersController.GetUserByUserType(userTypeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task DeleteUserById_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var userDto = new UserDTO { UserId = userId };
            _userServiceMock.Setup(s => s.DeleteUserByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _usersController.DeleteUserById(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("User deleted successfully."));
        }

        [Test]
        public async Task GetUserById_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var userDTO = new UserDTO { UserId = userId, Username = "user1" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userDTO);

            // Act
            var result = await _usersController.GetUserById(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(userDTO));
        }

        [TearDown]
        public void Teardown()
        {
            // Cleanup if needed
        }
    }
}