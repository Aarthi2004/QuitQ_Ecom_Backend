using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
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
        public async Task Register_ValidUser_ReturnsOk()
        {
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

            var result = await _usersController.Register(userDTO);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(userDTO));
        }

        [Test]
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _usersController.ModelState.AddModelError("Username", "Required");
            var userDTO = new UserDTO();

            // Act
            var result = await _usersController.Register(userDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAllUsers_ReturnsOk()
        {
            var users = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Username = "user1" },
                new UserDTO { UserId = 2, Username = "user2" }
            };
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _usersController.GetAllUsers();

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task GetUserByUserType_ReturnsOk()
        {
            int userTypeId = 1;
            var users = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Username = "user1", UserTypeId = userTypeId },
                new UserDTO { UserId = 2, Username = "user2", UserTypeId = userTypeId }
            };
            _userServiceMock.Setup(s => s.GetUsersByUserType(userTypeId)).ReturnsAsync(users);

            var result = await _usersController.GetUserByUserType(userTypeId);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task DeleteUserById_ReturnsOk()
        {
            int userId = 1;
            var userDto = new UserDTO { UserId = userId };
            _userServiceMock.Setup(s => s.DeleteUserByIdAsync(userId)).ReturnsAsync(userDto);

            var result = await _usersController.DeleteUserById(userId);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("User deleted successfully."));
        }

        [Test]
        public async Task GetUserById_ReturnsOk()
        {
            int userId = 1;
            var userDTO = new UserDTO { UserId = userId, Username = "user1" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userDTO);

            var result = await _usersController.GetUserById(userId);

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
