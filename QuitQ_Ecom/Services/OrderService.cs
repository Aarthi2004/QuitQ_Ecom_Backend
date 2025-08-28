using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrder _orderRepo;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrder orderRepo, ILogger<OrderService> logger)
        {
            _orderRepo = orderRepo;
            _logger = logger;
        }

        public async Task<Dictionary<bool, string>> PlaceOrder(int userId, string paymentType)
        {
            return await _orderRepo.PlaceOrder(userId, paymentType);
        }

        public async Task<List<OrderDTO>> GetOrdersByUserId(int userId)
        {
            return await _orderRepo.ViewAllOrdersByUserId(userId);
        }

        public async Task<OrderDTO> GetOrderById(int orderId)
        {
            return await _orderRepo.ViewOrderByOrderId(orderId);
        }

        public async Task<List<OrderDTO>> GetAllOrders()
        {
            return await _orderRepo.ViewAllOrders();
        }
    }
}