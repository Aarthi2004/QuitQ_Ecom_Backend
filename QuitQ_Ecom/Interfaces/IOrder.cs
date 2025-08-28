using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IOrder
    {
        Task<Dictionary<bool, string>> PlaceOrder(int UserId, string paymentType);
        Task<List<OrderDTO>> ViewAllOrdersByUserId(int userId);
        Task<OrderDTO> ViewOrderByOrderId(int orderId);
        Task<List<OrderDTO>> ViewOrdersByStoreId(int storeId);
        Task<List<OrderDTO>> ViewAllOrders();
    }
}