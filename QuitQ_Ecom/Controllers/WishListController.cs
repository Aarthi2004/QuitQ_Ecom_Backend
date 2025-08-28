using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Repository;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Service;
using System.Security.Claims;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishListService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishListService, ILogger<WishlistController> logger)
        {
            _wishListService = wishListService;
            _logger = logger;
        }

        // Changed route to not include userId, as it will be fetched from the token
        [HttpGet]
        public async Task<IActionResult> GetUserWishList()
        {
            try
            {
                // Get userId from the authenticated token
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var wishList = await _wishListService.GetUserWishList(userId);
                return Ok(wishList);
            }
            catch (GetUserWishlistException ex)
            {
                _logger.LogError(ex, $"An error occurred while getting user wish list for user ID: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while getting user wish list.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> AddToWishList([FromBody] WishListDTO wishListDTO)
        {
            try
            {
                // Get userId from the authenticated token
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                // CRITICAL: Overwrite the userId from the DTO with the secure userId from the token
                wishListDTO.UserId = userId;

                var addedWishList = await _wishListService.AddToWishList(wishListDTO);
                return Ok(addedWishList);
            }
            catch (AddToWishlistException ex)
            {
                _logger.LogError(ex, $"An error occurred while adding to wishlist: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding to wishlist.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // Changed route to not include userId, as it will be fetched from the token
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishList(int productId)
        {
            try
            {
                // Get userId from the authenticated token
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var isRemoved = await _wishListService.RemoveFromWishList(userId, productId);
                if (isRemoved)
                {
                    return Ok("Item removed from wishlist");
                }
                return NotFound("Item not found in wishlist");
            }
            catch (RemoveFromWishlistException ex)
            {
                _logger.LogError(ex, $"An error occurred while removing item from wishlist: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while removing item from wishlist.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}