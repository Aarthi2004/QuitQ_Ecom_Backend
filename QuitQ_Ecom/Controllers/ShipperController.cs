using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/shipment")]
    [ApiController]
    [Authorize]
    public class ShipperController : ControllerBase
    {
        private readonly IShipper _shipperService;
        private readonly ILogger<ShipperController> _logger;

        public ShipperController(IShipper shipperService, ILogger<ShipperController> logger)
        {
            _shipperService = shipperService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var list = await _shipperService.GetAllItems();
                if (list == null)
                {
                    return NotFound();
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting all items: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-order/{orderId:int}")]
        public async Task<IActionResult> GetShipItemByOrderId(int orderId)
        {
            try
            {
                var shipperObj = await _shipperService.GetShipperItemByOrderId(orderId);
                if (shipperObj == null)
                {
                    return NotFound();
                }
                return Ok(shipperObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting ship item by Order ID: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateorder")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDTO obj)
        {
            try
            {
                var status = await _shipperService.UpdateShipperOrderStatusByOrderId(obj.orderId, obj.orderStatus);
                if (status)
                {
                    return Ok("Successfully updated order status");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating order status: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{shipId:int}")]
        public async Task<IActionResult> GetShipItemById(int shipId)
        {
            try
            {
                var shipperObj = await _shipperService.GetShipperItemById(shipId);
                if (shipperObj == null)
                {
                    return NotFound();
                }
                return Ok(shipperObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting ship item by ID: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generateotp/{shipId:int}")]
        public async Task<IActionResult> GenerateOtp(int shipId)
        {
            try
            {
                var otpStatus = await _shipperService.GenerateOtpAtCustomer(shipId);
                if (!otpStatus)
                {
                    return NotFound();
                }
                return Ok(otpStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating OTP: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("validateotp")]
        public async Task<IActionResult> ValidateOtp([FromBody] ShipperDTO data)
        {
            try
            {
                var res = await _shipperService.ValidateOtp(data.ShipperId, data.ShipperName);
                if (!res)
                {
                    return NotFound();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while validating OTP: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-delivery-status")]
        public async Task<IActionResult> UpdateDeliveryStatus([FromBody] DeliverDTO data)
        {
            try
            {
                var res = await _shipperService.UpdateShipperOrderStatusByOrderId(data.OrderId, data.OrderStatus);
                if (res)
                    return Ok(res);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating delivery status: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}