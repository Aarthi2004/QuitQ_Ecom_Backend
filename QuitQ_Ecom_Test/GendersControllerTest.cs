using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class GendersControllerTest
    {
        private Mock<IGenderService> _genderServiceMock;
        private Mock<ILogger<GendersController>> _loggerMock;
        private GendersController _controller;

        [SetUp]
        public void Setup()
        {
            _genderServiceMock = new Mock<IGenderService>();
            _loggerMock = new Mock<ILogger<GendersController>>();
            _controller = new GendersController(_genderServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllGenders_WhenCalled_ReturnsOkWithList()
        {
            // Arrange
            var genders = new List<GenderDTO>
            {
                new GenderDTO { GenderId = 1, GenderName = "Male" }
            };
            _genderServiceMock.Setup(s => s.GetAllGenders()).ReturnsAsync(genders);

            // Act
            var result = await _controller.GetAllGenders();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var model = okResult.Value as List<GenderDTO>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
        }

        [Test]
        public async Task GetGenderById_ExistingId_ReturnsOkWithGender()
        {
            int genderId = 1;
            var gender = new GenderDTO { GenderId = genderId, GenderName = "Male" };
            _genderServiceMock.Setup(s => s.GetGenderById(genderId)).ReturnsAsync(gender);

            var result = await _controller.GetGenderById(genderId);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedGender = okResult.Value as GenderDTO;
            Assert.IsNotNull(returnedGender);
            Assert.AreEqual(genderId, returnedGender.GenderId);
        }

        [Test]
        public async Task GetGenderById_NonExistingId_ReturnsNotFound()
        {
            int genderId = 99;
            _genderServiceMock.Setup(s => s.GetGenderById(genderId)).ReturnsAsync((GenderDTO)null);

            var result = await _controller.GetGenderById(genderId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task AddGender_ValidData_ReturnsCreatedAtAction()
        {
            var genderDto = new GenderDTO { GenderId = 1, GenderName = "Male" };
            _genderServiceMock.Setup(s => s.AddGender(genderDto)).ReturnsAsync(genderDto);

            var result = await _controller.AddGender(genderDto);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(nameof(_controller.GetGenderById), createdResult.ActionName);
            Assert.AreEqual(genderDto.GenderId, createdResult.RouteValues["genderId"]);
        }

        [Test]
        public async Task UpdateGender_ValidData_ReturnsOkWithUpdatedGender()
        {
            int genderId = 1;
            var genderDto = new GenderDTO { GenderId = genderId, GenderName = "Male" };
            _genderServiceMock.Setup(s => s.UpdateGender(genderDto)).ReturnsAsync(genderDto);

            var result = await _controller.UpdateGender(genderId, genderDto);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var updatedGender = okResult.Value as GenderDTO;
            Assert.IsNotNull(updatedGender);
            Assert.AreEqual(genderId, updatedGender.GenderId);
        }

        [Test]
        public async Task UpdateGender_IdMismatch_ReturnsBadRequest()
        {
            var dto = new GenderDTO { GenderId = 2, GenderName = "Male" };

            var result = await _controller.UpdateGender(1, dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task DeleteGender_ExistingId_ReturnsOk()
        {
            int genderId = 1;
            _genderServiceMock.Setup(s => s.DeleteGender(genderId)).ReturnsAsync(true);

            var result = await _controller.DeleteGender(genderId);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteGender_NonExistingId_ReturnsNotFound()
        {
            int genderId = 1;
            _genderServiceMock.Setup(s => s.DeleteGender(genderId)).ReturnsAsync(false);

            var result = await _controller.DeleteGender(genderId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
