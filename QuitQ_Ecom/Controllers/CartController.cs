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

        // Removed userId from the route for security
        [HttpGet]
        public async Task<IActionResult> GetUserCartItems()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
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
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var addedCartItem = await _cartService.AddProductToCart(cartItem, userId);
                return Ok(addedCartItem);
            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product not found while adding to cart: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (InsufficientStockException ex)
            {
                _logger.LogWarning(ex, "Insufficient stock for product: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product to cart: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("increase-quantity/{cartItemId:int}")]
        public async Task<IActionResult> IncreaseProductQuantity(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var success = await _cartService.IncreaseProductQuantity(cartItemId, userId);
                if (success)
                    return Ok("Product quantity increased successfully");
                else
                    return NotFound("Cart item not found or does not belong to the user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while increasing product quantity: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("decrease-quantity/{cartItemId:int}")]
        public async Task<IActionResult> DecreaseProductQuantity(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var success = await _cartService.DecreaseProductQuantity(cartItemId, userId);
                if (success)
                    return Ok("Product quantity decreased successfully");
                else
                    return NotFound("Cart item not found or does not belong to the user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while decreasing product quantity: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Removed userId from the route for security
        [HttpGet("totalcost")]
        public async Task<IActionResult> GetCartTotalCost()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                decimal totalCost = await _cartService.GetCartTotalCost(userId);
                return Ok(totalCost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart total cost: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{cartItemId:int}")]
        public async Task<IActionResult> DeleteProductFromCart(int cartItemId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var status = await _cartService.RemoveProductFromCart(cartItemId, userId);
                if (status)
                {
                    return Ok("Item removed from cart");
                }
                return NotFound("Item not found in cart or does not belong to the user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the product from the cart: {Message}", ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}