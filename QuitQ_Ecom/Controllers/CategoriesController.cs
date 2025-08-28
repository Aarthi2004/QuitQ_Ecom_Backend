using Microsoft.AspNetCore.Mvc;
using QuitQ_Ecom.DTOs;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Exceptions;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        // CHANGED: The controller now depends on the Service layer
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving categories: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(categoryId);
                if (category == null)
                    return NotFound($"Category with ID {categoryId} not found.");

                return Ok(category);
            }
            catch (CategoryNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the category: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // CORRECTED: Role casing to match JWT token
        [HttpPost("")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                var addedCategory = await _categoryService.AddCategory(categoryDTO);
                Log.Information("Added Category is =>{@addedCategory}", addedCategory);
                return CreatedAtAction(nameof(GetCategoryById), new { categoryId = addedCategory.CategoryId }, addedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding the category: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // CORRECTED: Role casing to match JWT token
        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                if (categoryId != categoryDTO.CategoryId)
                    return BadRequest("Category ID mismatch.");

                var updatedCategory = await _categoryService.UpdateCategory(categoryDTO);
                return Ok(updatedCategory);
            }
            catch (CategoryNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the category: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // CORRECTED: Role casing to match JWT token
        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var deleted = await _categoryService.DeleteCategory(categoryId);
                if (!deleted)
                    return NotFound($"Category with ID {categoryId} not found.");

                return Ok($"Category with ID {categoryId} deleted successfully.");
            }
            catch (CategoryNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the category: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{categoryId}/subcategories")]
        public async Task<IActionResult> GetSubcategoriesByCategory(int categoryId)
        {
            try
            {
                var subcategories = await _categoryService.GetSubCategoriesByCategoryId(categoryId);
                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving subcategories: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{categoryId}/products")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _categoryService.GetProductsByCategory(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving products: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}