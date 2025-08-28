using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IOrderItem
    {
        Task<bool> AddNewOrderItem(List<CartDTO> cartItems, OrderDTO orderObj);
    }
}
