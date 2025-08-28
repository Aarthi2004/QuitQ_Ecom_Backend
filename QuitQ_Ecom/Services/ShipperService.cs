using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class ShipperService : IShipperService
    {
        private readonly IShipper _shipperRepo;
        private readonly ILogger<ShipperService> _logger;

        public ShipperService(IShipper shipperRepo, ILogger<ShipperService> logger)
        {
            _shipperRepo = shipperRepo;
            _logger = logger;
        }

        public async Task<List<ShipperDTO>> GetAllItems()
        {
            return await _shipperRepo.GetAllItems();
        }

        public async Task<ShipperDTO> GetShipperItemById(int id)
        {
            return await _shipperRepo.GetShipperItemById(id);
        }

        public async Task<bool> UpdateShipperOrderStatusByOrderId(int id, string deliveryStatus)
        {
            return await _shipperRepo.UpdateShipperOrderStatusByOrderId(id, deliveryStatus);
        }

        public async Task<bool> GenerateOtpAtCustomer(int shipperId)
        {
            return await _shipperRepo.GenerateOtpAtCustomer(shipperId);
        }

        public async Task<bool> ValidateOtp(int shipperId, string otp)
        {
            return await _shipperRepo.ValidateOtp(shipperId, otp);
        }
    }
}
