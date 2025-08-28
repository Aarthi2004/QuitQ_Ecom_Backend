using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuitQ_Ecom.Controllers;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces; // UPDATED: Changed repository to service
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuitQ_Ecom.Exceptions;

namespace QuitQ_Ecom_Test
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private CategoriesController _categoriesController;
        private Mock<ICategoryService> _categoryServiceMock; // UPDATED: Changed to mock the service
        private Mock<ILogger<CategoriesController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _categoryServiceMock = new Mock<ICategoryService>(); // UPDATED: Changed to mock the service
            _loggerMock = new Mock<ILogger<CategoriesController>>();
            // UPDATED: Constructor now uses the mocked service
            _categoriesController = new CategoriesController(_categoryServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllCategories_ValidData_ReturnsOk()
        {
            // Arrange
            _categoryServiceMock.Setup(service => service.GetAllCategories()).ReturnsAsync(new List<CategoryDTO>());

            // Act
            var result = await _categoriesController.GetAllCategories();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task GetCategoryById_ValidId_ReturnsOk()
        {
            // Arrange
            int categoryId = 1;
            _categoryServiceMock.Setup(service => service.GetCategoryById(categoryId)).ReturnsAsync(new CategoryDTO { CategoryId = categoryId, CategoryName = "TestCategory" });

            // Act
            var result = await _categoriesController.GetCategoryById(categoryId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task GetCategoryById_NotFound_ReturnsNotFound()
        {
            // Arrange
            int categoryId = 99;
            _categoryServiceMock.Setup(service => service.GetCategoryById(categoryId)).ThrowsAsync(new CategoryNotFoundException($"Category with ID {categoryId} not found."));

            // Act
            var result = await _categoriesController.GetCategoryById(categoryId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual($"Category with ID {categoryId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task AddCategory_ValidData_ReturnsCreated()
        {
            // Arrange
            var categoryDTO = new CategoryDTO { CategoryId = 1, CategoryName = "TestCategory" };
            _categoryServiceMock.Setup(service => service.AddCategory(categoryDTO)).ReturnsAsync(categoryDTO);

            // Act
            var result = await _categoriesController.AddCategory(categoryDTO);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.AreEqual(nameof(CategoriesController.GetCategoryById), createdResult.ActionName);
        }

        [Test]
        public async Task UpdateCategory_ValidData_ReturnsOk()
        {
            // Arrange
            int categoryId = 1;
            var categoryDTO = new CategoryDTO { CategoryId = categoryId, CategoryName = "TestCategory" };
            _categoryServiceMock.Setup(service => service.UpdateCategory(categoryDTO)).ReturnsAsync(categoryDTO);

            // Act
            var result = await _categoriesController.UpdateCategory(categoryId, categoryDTO);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(categoryDTO, okResult.Value);
        }

        [Test]
        public async Task DeleteCategory_ValidId_ReturnsOk()
        {
            // Arrange
            int categoryId = 1;
            _categoryServiceMock.Setup(service => service.DeleteCategory(categoryId)).ReturnsAsync(true);

            // Act
            var result = await _categoriesController.DeleteCategory(categoryId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"Category with ID {categoryId} deleted successfully.", okResult.Value);
        }

        [Test]
        public async Task GetSubcategoriesByCategory_ValidId_ReturnsOk()
        {
            // Arrange
            int categoryId = 1;
            _categoryServiceMock.Setup(service => service.GetSubCategoriesByCategoryId(categoryId)).ReturnsAsync(new List<SubCategoryDTO>());

            // Act
            var result = await _categoriesController.GetSubcategoriesByCategory(categoryId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [Test]
        public async Task GetProductsByCategory_ValidId_ReturnsOk()
        {
            // Arrange
            int categoryId = 1;
            _categoryServiceMock.Setup(service => service.GetProductsByCategory(categoryId)).ReturnsAsync(new List<ProductDTO>());

            // Act
            var result = await _categoriesController.GetProductsByCategory(categoryId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }
    }
}