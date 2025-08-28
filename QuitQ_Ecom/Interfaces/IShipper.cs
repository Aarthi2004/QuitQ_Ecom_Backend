using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IShipper
    {
        Task<List<ShipperDTO>> GetAllItems();
        Task<ShipperDTO> GetShipperItemById(int id);
        Task<bool> UpdateShipperOrderStatusByOrderId(int id, string deliveryStatus);
        Task<bool> GenerateOtpAtCustomer(int shipperId);
        Task<bool> ValidateOtp(int shipperid, string otp);
    }
}
