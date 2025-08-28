using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class ShipperRepositoryImpl : IShipper
    {
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ShipperRepositoryImpl> _logger;

        public ShipperRepositoryImpl(QuitQEcomContext quitQEcomContext, IMapper mapper, ILogger<ShipperRepositoryImpl> logger)
        {
            _context = quitQEcomContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> GenerateOtpAtCustomer(int shipperId)
        {
            try
            {
                var shipperObj = await _context.Shippers.FindAsync(shipperId);
                if (shipperObj == null) return false;

                Random rand = new Random();
                int otp = rand.Next(100000, 999999);
                shipperObj.ShipperName = otp.ToString();
                _context.Shippers.Update(shipperObj);
                await _context.SaveChangesAsync();

                var orderObj = await _context.Orders.FindAsync(shipperObj.OrderId);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == orderObj.UserId);
                if (user == null) return false;

                // TODO: Implement email sending with OTP here

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while generating OTP for shipper ID {shipperId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<ShipperDTO>> GetAllItems()
        {
            try
            {
                var obj = await _context.Shippers.ToListAsync();
                if (obj == null) return null;

                return _mapper.Map<List<ShipperDTO>>(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all shippers: {ex.Message}");
                return null;
            }
        }

        public async Task<ShipperDTO> GetShipperItemById(int id)
        {
            try
            {
                var shipperObj = await _context.Shippers.FindAsync(id);
                if (shipperObj == null) return null;

                return _mapper.Map<ShipperDTO>(shipperObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving shipper by ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateShipperOrderStatusByOrderId(int id, string deliveryStatus)
        {
            try
            {
                var orderObj = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
                if (orderObj == null) return false;

                orderObj.OrderStatus = deliveryStatus;
                _context.Update(orderObj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating shipper order status for order ID {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidateOtp(int shipperId, string otp)
        {
            try
            {
                var shipperObj = await _context.Shippers.FindAsync(shipperId);
                if (shipperObj == null) return false;

                if (shipperObj.ShipperName == otp)
                {
                    var orderObj = await _context.Orders.FindAsync(shipperObj.OrderId);
                    if (orderObj == null) return false;

                    orderObj.OrderStatus = "delivered";
                    _context.Update(orderObj);

                    // TODO: Update payment status if needed

                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating OTP for shipper ID {shipperId}: {ex.Message}");
                return false;
            }
        }
    }
}
