using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrder orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("all/{userId:int}")]
        public async Task<IActionResult> GetOrdersOfUser(int userId)
        {
            try
            {
                var res = await _orderService.ViewAllOrdersByUserId(userId);
                if (res == null || res.Count == 0)
                {
                    _logger.LogInformation($"No orders found for user with ID {userId}");
                    return NoContent();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching orders for user with ID {userId}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("store/{storeId:int}")]
        public async Task<IActionResult> GetOrdersByStore(int storeId)
        {
            try
            {
                var orders = await _orderService.ViewOrdersByStoreId(storeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching orders for store with ID {storeId}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderByOrderId(int orderId)
        {
            try
            {
                var order = await _orderService.ViewOrderByOrderId(orderId);
                if (order != null)
                {
                    return Ok(order);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching order ID {orderId}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.ViewAllOrders();
                if (orders == null || orders.Count == 0)
                {
                    _logger.LogInformation("No orders found in the system.");
                    return NoContent();
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all orders.");
                return BadRequest(ex.Message);
            }
        }
    }
}