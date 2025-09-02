using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models; // Added for User model
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IUser
    {
        Task<UserDTO> AddUser(UserDTO user);
        Task<UserDTO> DeleteUserById(int userId);
        Task<List<UserDTO>> GetUsersByUserType(int usertypeId);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);

        // --- NEW METHODS START ---
        Task<User> FindUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(User user);
        // --- NEW METHODS END ---
    }
}