using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repository
{
    public interface IWishlistRepository
    {
        Task<List<WishList>> GetUserWishList(int userId);
        Task<WishList> AddToWishList(WishList wishList);
        Task<bool> RemoveFromWishList(int userId, int productId);
    }
}