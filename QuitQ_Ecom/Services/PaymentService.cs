using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPayment _paymentRepo;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPayment paymentRepo, ILogger<PaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _logger = logger;
        }

        public async Task<PaymentDTO> AddNewPayment(OrderDTO order, string paymentType)
        {
            // Only COD supported: simply add payment record with "pending" status
            return await _paymentRepo.AddNewPayment(order, paymentType);
        }
    }
}
