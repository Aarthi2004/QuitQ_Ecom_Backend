using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Repository;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Exceptions;
using System.Security.Claims;
using QuitQ_Ecom.Service;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize(Roles = "Admin,Seller,Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCartItems()
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

                var cartItems = await _cartService.GetUserCartItems(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user cart items: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToCart([FromBody] CartDTO cartItem)
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

                var addedCartItem = await _cartService.AddProductToCart(cartItem, userId);
                return Ok(addedCartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product to cart: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Apply the same fix to all other methods in this controller...
        [HttpPost("increase-quantity/{cartItemId:int}")]
        public async Task<IActionResult> IncreaseProductQuantity(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId)) { return Unauthorized("User ID not found in token."); }

                var success = await _cartService.IncreaseProductQuantity(cartItemId, userId);
                if (success) return Ok("Product quantity increased successfully");
                return NotFound("Cart item not found or does not belong to the user.");
            }
            catch (Exception ex) { /* ... */ return StatusCode(500, "Internal server error"); }
        }

        [HttpPost("decrease-quantity/{cartItemId:int}")]
        public async Task<IActionResult> DecreaseProductQuantity(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId)) { return Unauthorized("User ID not found in token."); }

                var success = await _cartService.DecreaseProductQuantity(cartItemId, userId);
                if (success) return Ok("Product quantity decreased successfully");
                return NotFound("Cart item not found or does not belong to the user.");
            }
            catch (Exception ex) { /* ... */ return StatusCode(500, "Internal server error"); }
        }

        [HttpGet("totalcost")]
        public async Task<IActionResult> GetCartTotalCost()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId)) { return Unauthorized("User ID not found in token."); }

                decimal totalCost = await _cartService.GetCartTotalCost(userId);
                return Ok(totalCost);
            }
            catch (Exception ex) { /* ... */ return StatusCode(500, "Internal server error"); }
        }

        [HttpDelete("delete/{cartItemId:int}")]
        public async Task<IActionResult> DeleteProductFromCart(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId)) { return Unauthorized("User ID not found in token."); }

                var status = await _cartService.RemoveProductFromCart(cartItemId, userId);
                if (status) return Ok("Item removed from cart");
                return NotFound("Item not found in cart or does not belong to the user.");
            }
            catch (Exception ex) { /* ... */ return StatusCode(500, "Internal server error"); }
        }
    }
}