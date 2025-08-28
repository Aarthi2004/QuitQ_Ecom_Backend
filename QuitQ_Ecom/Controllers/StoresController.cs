using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly ILogger<StoresController> _logger;

        public StoresController(IStoreService storeService, ILogger<StoresController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllStores()
        {
            try
            {
                var stores = await _storeService.GetAllStores();
                return Ok(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all stores.");
                return StatusCode(500, "An unexpected error occurred while retrieving stores.");
            }
        }

        [HttpGet("{storeId:int}")]
        public async Task<IActionResult> GetStoreById([FromRoute] int storeId)
        {
            try
            {
                var store = await _storeService.GetStoreById(storeId);
                if (store == null)
                    return NotFound();
                return Ok(store);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting store by ID {storeId}.");
                return StatusCode(500, "An unexpected error occurred while retrieving the store.");
            }
        }

        [Authorize(Roles = "Seller,Admin")] // CORRECTED: Casing to match your JWT token
        [HttpPost("")]
        public async Task<IActionResult> AddStore([FromForm] StoreDTO storeDTO)
        {
            try
            {
                // CORRECTED: Claim name to match your JWT token
                var userIdClaim = User.FindFirst("UserId");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int sellerId))
                {
                    _logger.LogWarning("AddStore attempt failed: User ID claim not found or invalid.");
                    return Unauthorized("User ID claim not found or invalid.");
                }

                if (storeDTO.StoreImageFile == null || storeDTO.StoreImageFile.Length == 0)
                {
                    _logger.LogWarning("AddStore attempt failed: Store image file is empty.");
                    return BadRequest("Store image file is required.");
                }

                storeDTO.SellerId = sellerId;

                var returnedObj = await _storeService.AddStore(storeDTO);
                if (returnedObj == null)
                {
                    _logger.LogError("Store service returned null after adding store.");
                    return StatusCode(500, "Failed to add store.");
                }

                return Ok("Store added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new store.");
                return StatusCode(500, $"Internal server error while adding store: {ex.Message}");
            }
        }

        [Authorize(Roles = "Seller,Admin")] // CORRECTED: Casing to match your JWT token
        [HttpPut("{storeId:int}")]
        public async Task<IActionResult> UpdateStore([FromRoute] int storeId, [FromForm] StoreDTO storeDTO)
        {
            try
            {
                // CORRECTED: Claim name to match your JWT token
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int sellerId))
                {
                    return Unauthorized("User ID claim not found or invalid.");
                }

                // The controller should not contain database or mapping logic.
                // It should call the service and handle the result.
                storeDTO.SellerId = sellerId;

                var returnedObj = await _storeService.UpdateStore(storeId, storeDTO);

                if (returnedObj == null)
                {
                    return NotFound();
                }

                return Ok("Store updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating store with ID {storeId}.");
                return StatusCode(500, $"Internal server error while updating store: {ex.Message}");
            }
        }

        [HttpDelete("{storeId:int}")]
        [Authorize(Roles = "Seller,Admin")] // CORRECTED: Casing to match your JWT token
        public async Task<IActionResult> DeleteStore([FromRoute] int storeId)
        {
            try
            {
                // CORRECTED: Claim name to match your JWT token
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int sellerId))
                {
                    return Unauthorized("User ID claim not found or invalid.");
                }

                var storeToDelete = await _storeService.GetStoreById(storeId);
                if (storeToDelete == null)
                {
                    return NotFound();
                }

                if (storeToDelete.SellerId != sellerId && !User.IsInRole("Admin"))
                {
                    return Forbid("You do not have permission to delete this store.");
                }

                var deleted = await _storeService.DeleteStore(storeId);
                if (!deleted)
                    return NotFound();

                return Ok("Store deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting store with ID {storeId}.");
                return StatusCode(500, "Internal server error while deleting store.");
            }
        }

        [HttpGet("userstores/{userId:int}")]
        [Authorize(Roles = "Seller,Admin")] // CORRECTED: Casing to match your JWT token
        public async Task<IActionResult> GetAllStoresOfUserByUserId(int userId)
        {
            try
            {
                var storeslist = await _storeService.GetAllStoresOfUserByUserId(userId);
                if (storeslist != null)
                {
                    return Ok(storeslist);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting stores for user with ID {userId}.");
                return StatusCode(500, "Internal server error while retrieving user stores.");
            }
        }
    }
}