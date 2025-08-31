using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System;
using System.Collections.Generic;
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

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost("")]
        public async Task<IActionResult> AddStore([FromForm] StoreDTO storeDTO)
        {
            try
            {
                int sellerIdToAssign;

                // --- THIS IS THE CORRECTED ROLE-BASED LOGIC ---
                // First, check the role of the user making the request.
                if (User.IsInRole("Admin"))
                {
                    // If the user is an Admin, we MUST use the SellerId that was provided in the form.
                    // This allows the Admin to create a store on behalf of any seller.
                    if (storeDTO.SellerId == null || storeDTO.SellerId <= 0)
                    {
                        return BadRequest("When creating a store as an Admin, a valid SellerId must be provided.");
                    }
                    sellerIdToAssign = storeDTO.SellerId.Value;
                }
                else // Otherwise, the user making the request must be a Seller
                {
                    // If the user is a Seller, we IGNORE any SellerId from the form for security.
                    // We can only allow them to create a store for themselves, using their own ID from the token.
                    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!int.TryParse(userIdClaim, out int tokenSellerId))
                    {
                        return Unauthorized("Your User ID could not be found in your token.");
                    }
                    sellerIdToAssign = tokenSellerId;
                }
                // --- END OF CORRECTED LOGIC ---

                if (storeDTO.StoreImageFile == null || storeDTO.StoreImageFile.Length == 0)
                {
                    return BadRequest("A store image file is required.");
                }

                // Now, we set the correctly determined sellerId on the DTO before sending it to the service.
                storeDTO.SellerId = sellerIdToAssign;

                var returnedObj = await _storeService.AddStore(storeDTO);
                if (returnedObj == null)
                {
                    return StatusCode(500, "An error occurred while adding the store.");
                }

                // Return the created object so the frontend can see the result.
                return Ok(returnedObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a new store.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPut("{storeId:int}")]
        public async Task<IActionResult> UpdateStore([FromRoute] int storeId, [FromForm] StoreDTO storeDTO)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int sellerId))
                {
                    return Unauthorized("User ID claim not found or invalid.");
                }

                // Security check: ensure the user owns this store or is an admin
                var storeToUpdate = await _storeService.GetStoreById(storeId);
                if (storeToUpdate == null) return NotFound();
                if (storeToUpdate.SellerId != sellerId && !User.IsInRole("Admin"))
                {
                    return Forbid("You do not have permission to update this store.");
                }

                // The service layer needs the sellerId from the token to ensure consistency
                storeDTO.SellerId = storeToUpdate.SellerId;

                var returnedObj = await _storeService.UpdateStore(storeId, storeDTO);
                if (returnedObj == null) return NotFound();
                return Ok(returnedObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating store with ID {storeId}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{storeId:int}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> DeleteStore([FromRoute] int storeId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int sellerId))
                {
                    return Unauthorized("User ID claim not found or invalid.");
                }

                var storeToDelete = await _storeService.GetStoreById(storeId);
                if (storeToDelete == null) return NotFound();

                // Security check: Only the owner or an Admin can delete the store.
                if (storeToDelete.SellerId != sellerId && !User.IsInRole("Admin"))
                {
                    return Forbid("You do not have permission to delete this store.");
                }

                var deleted = await _storeService.DeleteStore(storeId);
                if (!deleted) return NotFound();
                return Ok("Store deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting store with ID {storeId}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("userstores/{userId:int}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> GetAllStoresOfUserByUserId(int userId)
        {
            try
            {
                // Security Check: ensure the requesting user is the one they're asking for, or an admin
                var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (requestingUserId != userId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid("You are not authorized to view these stores.");
                }

                var storeslist = await _storeService.GetAllStoresOfUserByUserId(userId);

                // Best practice: return 200 OK with an empty list if nothing is found
                return Ok(storeslist ?? new List<StoreDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting stores for user with ID {userId}.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}