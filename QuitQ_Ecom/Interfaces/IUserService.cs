using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUser(UserDTO user);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<List<UserDTO>> GetUsersByUserType(int typeId);
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<UserDTO> DeleteUserByIdAsync(int id);
    }
}
