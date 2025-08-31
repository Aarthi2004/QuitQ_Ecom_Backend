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
using System.Security.Claims; // This should already be here

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

        [HttpGet]
        public async Task<IActionResult> GetUserWishList()
        {
            try
            {
                // --- FIX IS HERE: Use the standard claim type ---
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // ---
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var wishList = await _wishListService.GetUserWishList(userId);
                return Ok(wishList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting user wish list.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> AddToWishList([FromBody] WishListDTO wishListDTO)
        {
            try
            {
                // --- FIX IS HERE: Use the standard claim type ---
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // ---
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                wishListDTO.UserId = userId; // Overwrite with secure ID from token
                var addedWishList = await _wishListService.AddToWishList(wishListDTO);
                return Ok(addedWishList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding to wishlist.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishList(int productId)
        {
            try
            {
                // --- FIX IS HERE: Use the standard claim type ---
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // ---
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while removing item from wishlist.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}