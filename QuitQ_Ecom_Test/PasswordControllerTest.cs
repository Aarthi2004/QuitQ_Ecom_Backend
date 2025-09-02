using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Interfaces;
using System.Threading.Tasks;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class PasswordControllerTest
    {
        private PasswordController _passwordController;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<PasswordController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<PasswordController>>();
            _passwordController = new PasswordController(_userServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task ResetPassword_ValidRequest_ReturnsOk()
        {
            // Arrange
            var resetDto = new ResetPasswordDTO { Email = "test@example.com", NewPassword = "newpassword123" };
            _userServiceMock.Setup(s => s.ResetPasswordAsync(resetDto)).ReturnsAsync(true);

            // Act
            var result = await _passwordController.ResetPassword(resetDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task ResetPassword_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var resetDto = new ResetPasswordDTO { Email = "nonexistent@example.com", NewPassword = "newpassword123" };
            _userServiceMock.Setup(s => s.ResetPasswordAsync(resetDto)).ThrowsAsync(new UserNotFoundException("User not found"));

            // Act
            var result = await _passwordController.ResetPassword(resetDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ResetPassword_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _passwordController.ModelState.AddModelError("Email", "The Email field is not a valid e-mail address.");
            var resetDto = new ResetPasswordDTO { Email = "invalid-email", NewPassword = "newpassword123" };

            // Act
            var result = await _passwordController.ResetPassword(resetDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
} 