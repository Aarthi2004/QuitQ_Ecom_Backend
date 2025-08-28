using QuitQ_Ecom.DTOs;

namespace QuitQ_Ecom.Service
{
    public interface ICartService
    {
        Task<List<CartDTO>> GetUserCartItems(int userId);
        Task<CartDTO> AddProductToCart(CartDTO cartItem, int userId);
        Task<bool> IncreaseProductQuantity(int cartItemId, int userId);
        Task<bool> DecreaseProductQuantity(int cartItemId, int userId);
        Task<decimal> GetCartTotalCost(int userId);
        Task<bool> RemoveProductFromCart(int cartItemId, int userId);
        Task<bool> RemoveCartItemsOfUser(int userId);
    }
}