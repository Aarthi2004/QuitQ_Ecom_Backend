using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IOrderService
    {
        Task<Dictionary<bool, string>> PlaceOrder(int userId, string paymentType);
        Task<List<OrderDTO>> GetOrdersByUserId(int userId);
        Task<OrderDTO> GetOrderById(int orderId);
        Task<List<OrderDTO>> GetAllOrders();
    }
}