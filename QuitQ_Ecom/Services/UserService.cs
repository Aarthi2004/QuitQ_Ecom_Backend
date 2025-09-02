using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models; // Required for User model

namespace QuitQ_Ecom.Services
{
    public class UserService : IUserService
    {
        private readonly IUser _userRepo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUser userRepo, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        // ... existing methods (HashPassword, RegisterUser, etc.) remain the same ...

        public static string HashPassword(string password)
        {
            using var sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        public async Task<UserDTO> RegisterUser(UserDTO user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Password))
                    throw new AddUserException("Password is required");

                user.Password = HashPassword(user.Password);
                user.UserStatusId = 1;
                return await _userRepo.AddUser(user);
            }
            catch (Exception ex) when (!(ex is AddUserException))
            {
                throw new AddUserException("Error occurred while registering user", ex);
            }
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepo.GetAllUsersAsync();
                if (users == null || users.Count == 0)
                    throw new GetAllUsersException("No users found");
                return users;
            }
            catch (Exception ex) when (!(ex is GetAllUsersException))
            {
                throw new GetAllUsersException("Error fetching all users", ex);
            }
        }

        public async Task<List<UserDTO>> GetUsersByUserType(int typeId)
        {
            try
            {
                var users = await _userRepo.GetUsersByUserType(typeId);
                if (users == null || users.Count == 0)
                    throw new GetUsersByUserTypeException($"No users found for type {typeId}");
                return users;
            }
            catch (Exception ex) when (!(ex is GetUsersByUserTypeException))
            {
                throw new GetUsersByUserTypeException($"Error fetching users for type {typeId}", ex);
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                throw new UserNotFoundException($"User with ID {id} not found");
            return user;
        }

        public async Task<UserDTO> DeleteUserByIdAsync(int id)
        {
            var deletedUser = await _userRepo.DeleteUserById(id);
            if (deletedUser == null)
                throw new UserNotFoundException($"User with ID {id} not found");
            return deletedUser;
        }

        // --- NEW METHOD IMPLEMENTATION START ---
        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userRepo.FindUserByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
            {
                throw new UserNotFoundException($"User with email '{resetPasswordDTO.Email}' not found.");
            }

            user.Password = HashPassword(resetPasswordDTO.NewPassword);
            return await _userRepo.UpdateUserAsync(user);
        }
        // --- NEW METHOD IMPLEMENTATION END ---
    }
}