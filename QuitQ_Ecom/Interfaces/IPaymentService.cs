using QuitQ_Ecom.DTOs;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDTO> AddNewPayment(OrderDTO order, string paymentType);
    }
}
