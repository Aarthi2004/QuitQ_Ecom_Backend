using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private ProductsController _productsController;
        private Mock<IProductService> _productServiceMock;
        private Mock<ILogger<ProductsController>> _loggerMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            // The controller no longer depends on IMapper, but the repository and service do.
            // For the test, we only need to mock what the controller directly depends on.
            _productsController = new ProductsController(_productServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SearchProducts_ReturnsOk()
        {
            string query = "test";
            var mockProducts = new List<ProductDTO>
            {
                new ProductDTO { ProductId = 1, ProductName = "Test Product 1", Price = 10 },
                new ProductDTO { ProductId = 2, ProductName = "Test Product 2", Price = 20 },
            };
            _productServiceMock.Setup(r => r.Search(query)).ReturnsAsync(mockProducts);
            var result = await _productsController.SearchProducts(query);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var products = okResult.Value as List<ProductDTO>;
            Assert.AreEqual(2, products.Count);
        }

        [Test]
        public async Task DeleteProductByID_ReturnsOk()
        {
            int productId = 1;
            _productServiceMock.Setup(r => r.Delete(productId)).ReturnsAsync(true);
            var result = await _productsController.DeleteProductByID(productId);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("product deleted Successfully", okResult.Value);
        }

        [Test]
        public async Task DeleteProductByID_ReturnsNotFound()
        {
            int productId = 123;
            _productServiceMock.Setup(r => r.Delete(productId)).ReturnsAsync(false);
            var result = await _productsController.DeleteProductByID(productId);
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
            Assert.AreEqual("Product not found", notFound.Value);
        }
    }
}