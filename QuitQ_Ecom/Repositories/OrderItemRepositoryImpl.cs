using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class OrderItemRepositoryImpl : IOrderItem
    {
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderItemRepositoryImpl> _logger;

        public OrderItemRepositoryImpl(QuitQEcomContext quitQEcomContext, IMapper mapper, ILogger<OrderItemRepositoryImpl> logger)
        {
            _context = quitQEcomContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddNewOrderItem(List<CartDTO> cartItems, OrderDTO orderObj)
        {
            try
            {
                foreach (var item in cartItems)
                {
                    var orderItemObj = new OrderItem()
                    {
                        OrderId = orderObj.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    };
                    await _context.OrderItems.AddAsync(orderItemObj);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new order items.");
                throw;
            }
        }
    }
}
