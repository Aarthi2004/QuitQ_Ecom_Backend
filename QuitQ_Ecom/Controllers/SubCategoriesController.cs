using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Services; // Changed to use the new Services folder
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/subcategories")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService; // Changed to use service
        private readonly ILogger<SubCategoriesController> _logger;

        public SubCategoriesController(ISubCategoryService subCategoryService, ILogger<SubCategoriesController> logger)
        {
            _subCategoryService = subCategoryService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            try
            {
                var subcategories = await _subCategoryService.GetAllSubCategories(); // Now calls the service
                if (subcategories != null)
                {
                    return Ok(subcategories);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retriving subcategories: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost("")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> AddSubCategory([FromBody] SubCategoryDTO subCategoryDTO)
        {
            try
            {
                var addedSubCategory = await _subCategoryService.AddSubCategory(subCategoryDTO); // Now calls the service
                return Ok(addedSubCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding subcategory: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            try
            {
                var subCategory = await _subCategoryService.GetSubCategoryById(id); // Now calls the service
                if (subCategory == null)
                    return NotFound();
                return Ok(subCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting subcategory by ID: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller ,Admin")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] SubCategoryDTO subCategoryDTO)
        {
            try
            {
                subCategoryDTO.SubCategoryId = id;
                var updatedSubCategory = await _subCategoryService.UpdateSubCategory(subCategoryDTO); // Now calls the service
                return Ok(updatedSubCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating subcategory: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            try
            {
                var result = await _subCategoryService.DeleteSubCategory(id); // Now calls the service
                if (!result)
                    return NotFound();
                return Ok("Subcategory deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting subcategory: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}