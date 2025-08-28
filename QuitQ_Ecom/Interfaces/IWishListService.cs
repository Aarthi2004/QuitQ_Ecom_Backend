using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Service
{
    public interface IWishlistService
    {
        Task<List<WishListDTO>> GetUserWishList(int userId);
        Task<WishListDTO> AddToWishList(WishListDTO wishListDTO);
        Task<bool> RemoveFromWishList(int userId, int productId);
    }
}