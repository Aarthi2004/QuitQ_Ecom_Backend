using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class StateControllerTests
    {
        private StateController _stateController;
        private Mock<IStateService> _stateServiceMock;
        private Mock<ILogger<StateController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _stateServiceMock = new Mock<IStateService>();
            _loggerMock = new Mock<ILogger<StateController>>();
            _stateController = new StateController(_stateServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllStates_ReturnsOk()
        {
            // Arrange
            var mockStates = new List<StateDTO>
            {
                new StateDTO { StateId = 1, StateName = "State 1" },
                new StateDTO { StateId = 2, StateName = "State 2" }
            };
            _stateServiceMock.Setup(service => service.GetAllStates()).ReturnsAsync(mockStates);

            // Act
            var result = await _stateController.GetAllStates();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var states = okResult.Value as List<StateDTO>;
            Assert.IsNotNull(states);
            Assert.AreEqual(mockStates.Count, states.Count);
        }

        [Test]
        public async Task GetStateById_ReturnsOk()
        {
            // Arrange
            int stateId = 1;
            var mockState = new StateDTO { StateId = stateId, StateName = "State 1" };
            _stateServiceMock.Setup(service => service.GetStateById(stateId)).ReturnsAsync(mockState);

            // Act
            var result = await _stateController.GetStateById(stateId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var state = okResult.Value as StateDTO;
            Assert.IsNotNull(state);
            Assert.AreEqual(stateId, state.StateId);
        }

        [Test]
        public async Task GetStateById_ReturnsNotFound_WhenStateNotFound()
        {
            // Arrange
            int stateId = 999;
            _stateServiceMock.Setup(service => service.GetStateById(stateId)).ThrowsAsync(new StateNotFoundException(stateId));

            // Act
            var result = await _stateController.GetStateById(stateId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("State not found", notFoundResult.Value);
        }

        [Test]
        public async Task AddState_ReturnsCreatedAtAction()
        {
            // Arrange
            var stateToAdd = new StateDTO { StateId = 1, StateName = "New State" };
            _stateServiceMock.Setup(service => service.AddState(stateToAdd)).ReturnsAsync(stateToAdd);

            // Act
            var result = await _stateController.AddState(stateToAdd);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(_stateController.GetStateById), createdAtActionResult.ActionName);
            var addedState = createdAtActionResult.Value as StateDTO;
            Assert.IsNotNull(addedState);
            Assert.AreEqual(stateToAdd.StateId, addedState.StateId);
        }

        [Test]
        public async Task UpdateState_ReturnsOk()
        {
            // Arrange
            int stateId = 1;
            var stateToUpdate = new StateDTO { StateId = stateId, StateName = "Updated State" };
            _stateServiceMock.Setup(service => service.UpdateState(stateId, stateToUpdate)).ReturnsAsync(stateToUpdate);

            // Act
            var result = await _stateController.UpdateState(stateId, stateToUpdate);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var updatedState = okResult.Value as StateDTO;
            Assert.IsNotNull(updatedState);
            Assert.AreEqual(stateId, updatedState.StateId);
            Assert.AreEqual(stateToUpdate.StateName, updatedState.StateName);
        }

        [Test]
        public async Task UpdateState_ReturnsNotFound_WhenStateNotFound()
        {
            // Arrange
            int stateId = 999;
            var stateToUpdate = new StateDTO { StateId = stateId, StateName = "Non-existent State" };
            _stateServiceMock.Setup(service => service.UpdateState(stateId, stateToUpdate)).ThrowsAsync(new StateNotFoundException(stateId));

            // Act
            var result = await _stateController.UpdateState(stateId, stateToUpdate);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("State not found", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteState_ReturnsOk()
        {
            // Arrange
            int stateId = 1;
            _stateServiceMock.Setup(service => service.DeleteState(stateId)).ReturnsAsync(true);

            // Act
            var result = await _stateController.DeleteState(stateId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("State deleted successfully", okResult.Value);
        }

        [Test]
        public async Task DeleteState_ReturnsNotFound_WhenStateNotFound()
        {
            // Arrange
            int stateId = 999;
            _stateServiceMock.Setup(service => service.DeleteState(stateId)).ThrowsAsync(new StateNotFoundException(stateId));

            // Act
            var result = await _stateController.DeleteState(stateId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("State not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetAllCitiesByStateId_ReturnsOk()
        {
            // Arrange
            int stateId = 1;
            var mockCities = new List<CityDTO>
            {
                new CityDTO { CityId = 1, CityName = "City A", StateId = stateId },
                new CityDTO { CityId = 2, CityName = "City B", StateId = stateId }
            };
            _stateServiceMock.Setup(service => service.GetCitiesByStateId(stateId)).ReturnsAsync(mockCities);

            // Act
            var result = await _stateController.GetAllCitiesByStateId(stateId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var cities = okResult.Value as List<CityDTO>;
            Assert.IsNotNull(cities);
            Assert.AreEqual(mockCities.Count, cities.Count);
        }
    }
}