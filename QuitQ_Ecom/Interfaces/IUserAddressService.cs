using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IUserAddressService
    {
        Task<UserAddressDTO> GetActiveUserAddressByUserId(int userId);
        Task<UserAddressDTO> AddUserAddress(UserAddressDTO userAddressDTO);
        Task<bool> DeleteUserAddress(int userAddressId);
        Task<List<UserAddressDTO>> GetUserAddressesByUserId(int userId);
        Task<UserAddressDTO> UpdateUserAddress(int userAddressId, UserAddressDTO userAddressDTO);
    }
}