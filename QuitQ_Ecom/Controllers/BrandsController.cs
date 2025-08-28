using Microsoft.AspNetCore.Mvc;
using QuitQ_Ecom.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using QuitQ_Ecom.Services;
using QuitQ_Ecom.Exceptions;
using System;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<BrandsController> _logger;

        public BrandsController(IBrandService brandService, ILogger<BrandsController> logger)
        {
            _brandService = brandService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllBrands()
        {
            try
            {
                var brands = await _brandService.GetAllBrands();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in GetAllBrands.");
                return StatusCode(500, "An internal server error occurred while retrieving all brands.");
            }
        }

        [HttpGet("{brandId}")]
        public async Task<IActionResult> GetBrandById(int brandId)
        {
            try
            {
                var brand = await _brandService.GetBrandById(brandId);
                if (brand == null)
                    return NotFound($"Brand with ID {brandId} not found.");

                return Ok(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in GetBrandById for ID {BrandId}.", brandId);
                return StatusCode(500, "An internal server error occurred while retrieving the brand.");
            }
        }

        [HttpPost("")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> AddBrand([FromForm] BrandDTO brandDTO)
        {
            try
            {
                if (brandDTO.BrandLogoImg == null || brandDTO.BrandLogoImg.Length == 0)
                {
                    return BadRequest("Brand logo image is required.");
                }

                var addedBrand = await _brandService.AddBrand(brandDTO);
                return CreatedAtAction(nameof(GetBrandById), new { brandId = addedBrand.BrandId }, "Brand added successfully");
            }
            catch (AddBrandException ex)
            {
                _logger.LogError(ex, "Business logic error adding brand: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occurred while adding the brand.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in AddBrand.");
                return StatusCode(500, "An internal server error occurred while adding the brand.");
            }
        }

        [HttpPut("{brandId}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> UpdateBrand([FromRoute] int brandId, [FromForm] BrandDTO brandDTO)
        {
            try
            {
                var updatedBrand = await _brandService.UpdateBrand(brandId, brandDTO);
                if (updatedBrand == null)
                {
                    return NotFound($"Brand with ID {brandId} not found.");
                }
                return Ok("Brand updated successfully");
            }
            catch (UpdateBrandException ex)
            {
                _logger.LogError(ex, "Business logic error updating brand with ID {BrandId}: {Message}", brandId, ex.Message);
                return StatusCode(500, "An internal server error occurred while updating the brand.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in UpdateBrand for ID {BrandId}.", brandId);
                return StatusCode(500, "An internal server error occurred while updating the brand.");
            }
        }

        [HttpDelete("{brandId}")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> DeleteBrand(int brandId)
        {
            try
            {
                var deleted = await _brandService.DeleteBrand(brandId);
                if (!deleted)
                    return NotFound($"Brand with ID {brandId} not found.");

                return Ok($"Brand with ID {brandId} deleted successfully.");
            }
            catch (DeleteBrandException ex)
            {
                _logger.LogError(ex, "Business logic error deleting brand with ID {BrandId}: {Message}", brandId, ex.Message);
                return StatusCode(500, "An internal server error occurred while deleting the brand.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred in DeleteBrand for ID {BrandId}.", brandId);
                return StatusCode(500, "An internal server error occurred while deleting the brand.");
            }
        }
    }
}