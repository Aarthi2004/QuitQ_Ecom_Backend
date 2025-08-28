using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class PaymentRepositoryImpl : IPayment
    {
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentRepositoryImpl> _logger;

        public PaymentRepositoryImpl(QuitQEcomContext quitQEcomContext, IMapper mapper, ILogger<PaymentRepositoryImpl> logger)
        {
            _context = quitQEcomContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaymentDTO> AddNewPayment(OrderDTO order, string paymentType)
        {
            try
            {
                // For COD, payment status is pending until delivery
                var paymentStatus = paymentType.ToLower() == "cod" ? "pending" : "completed";

                var paymentObj = new Models.Payment()
                {
                    OrderId = order.OrderId,
                    Amount = order.TotalAmount,
                    PaymentMethod = paymentType,
                    PaymentStatus = paymentStatus,
                    PaymentDate = paymentType.ToLower() == "cod" ? DateTime.MinValue : DateTime.Now
                };

                await _context.Payments.AddAsync(paymentObj);
                await _context.SaveChangesAsync();

                return _mapper.Map<PaymentDTO>(paymentObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while adding payment record for Order ID {order.OrderId}: {ex.Message}");
                return null;
            }
        }
    }
}
