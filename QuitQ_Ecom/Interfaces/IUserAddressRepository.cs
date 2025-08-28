using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IUserAddressRepository
    {
        Task<UserAddress> GetActiveUserAddressByUserId(int userId);
        Task<UserAddress> AddUserAddress(UserAddress userAddress);
        Task<bool> DeleteUserAddress(int userAddressId);
        Task<List<UserAddress>> GetUserAddressesByUserId(int userId);
        Task<UserAddress> UpdateUserAddress(int userAddressId, UserAddress userAddress);
    }
}