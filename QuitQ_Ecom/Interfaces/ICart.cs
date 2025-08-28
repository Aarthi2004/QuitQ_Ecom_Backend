using QuitQ_Ecom.Models;

namespace QuitQ_Ecom.Repository
{
    public interface ICart
    {
        Task<List<Cart>> GetUserCartItems(int userId);
        Task<Cart> GetCartItemByProductIdAndUserId(int userId, int productId);
        Task<Cart> AddNewProductToCart(Cart cartItem);
        Task<bool> IncreaseProductQuantity(int cartItemId);
        Task<bool> DecreaseProductQuantity(int cartItemId);
        Task<bool> RemoveProductFromCart(int cartItemId);
        Task<bool> RemoveCartItemsOfUser(int userId);
        Task<decimal> GetTotalCartCost(int userId);
        Task<Cart> GetCartItemById(int cartItemId);
        Task<bool> UpdateCartItem(Cart cartItem); // New method
    }
}