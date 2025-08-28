using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/checkout")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IOrder _orderService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IOrder orderService, ILogger<CheckoutController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // Place order with COD only
        [HttpPost("place-order")]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var userId = request.UserId;

                var result = await _orderService.PlaceOrder(userId, "cod"); // only COD here

                if (result.ContainsKey(true))
                {
                    return Ok(new { message = result[true] });
                }
                else
                {
                    return BadRequest(new { message = result[false] });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to place order for user {request.UserId}: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }

    public class PlaceOrderRequest
    {
        public int UserId { get; set; }
    }
}
