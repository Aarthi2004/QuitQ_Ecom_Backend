using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class StoresControllerTests
    {
        private StoresController _storesController;
        private Mock<IStoreService> _storeServiceMock;
        private ILogger<StoresController> _logger;

        [SetUp]
        public void Setup()
        {
            _storeServiceMock = new Mock<IStoreService>();
            _logger = Mock.Of<ILogger<StoresController>>();

            _storesController = new StoresController(
                _storeServiceMock.Object,
                _logger
            );
        }

        [Test]
        public async Task GetAllStores_ReturnsOk()
        {
            // Arrange
            var stores = new List<StoreDTO>
            {
                new StoreDTO { StoreId = 1, StoreName = "Store 1" },
                new StoreDTO { StoreId = 2, StoreName = "Store 2" }
            };
            _storeServiceMock.Setup(service => service.GetAllStores()).ReturnsAsync(stores);

            // Act
            var result = await _storesController.GetAllStores();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedStores = okResult.Value as List<StoreDTO>;
            Assert.IsNotNull(returnedStores);
            Assert.AreEqual(stores.Count, returnedStores.Count);
        }

        [Test]
        public async Task GetStoreById_ExistingId_ReturnsOk()
        {
            // Arrange
            int storeId = 1;
            var store = new StoreDTO { StoreId = storeId, StoreName = "Store 1" };
            _storeServiceMock.Setup(service => service.GetStoreById(storeId)).ReturnsAsync(store);

            // Act
            var result = await _storesController.GetStoreById(storeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedStore = okResult.Value as StoreDTO;
            Assert.IsNotNull(returnedStore);
            Assert.AreEqual(store.StoreId, returnedStore.StoreId);
        }

        [Test]
        public async Task GetStoreById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            int storeId = 1;
            _storeServiceMock.Setup(service => service.GetStoreById(storeId)).ReturnsAsync((StoreDTO)null);

            // Act
            var result = await _storesController.GetStoreById(storeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Add more tests for the other controller methods here
    }
}