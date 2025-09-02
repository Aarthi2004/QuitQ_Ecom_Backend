using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class UserRepositoryImpl : IUser
    {
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepositoryImpl> _logger;

        public UserRepositoryImpl(QuitQEcomContext quitQEcomContext, IMapper mapper, ILogger<UserRepositoryImpl> logger)
        {
            _context = quitQEcomContext;
            _mapper = mapper;
            _logger = logger;
        }

        // ... existing methods (AddUser, DeleteUserById, etc.) remain the same ...

        public async Task<UserDTO> AddUser(UserDTO userDto)
        {
            try
            {
                var userEntity = _mapper.Map<User>(userDto);
                await _context.Users.AddAsync(userEntity);
                await _context.SaveChangesAsync();
                return _mapper.Map<UserDTO>(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding user");
                throw;
            }
        }

        public async Task<UserDTO> DeleteUserById(int userId)
        {
            try
            {
                var userobj = await _context.Users.FindAsync(userId);
                if (userobj == null) return null;
                _context.Users.Remove(userobj);
                await _context.SaveChangesAsync();
                return _mapper.Map<UserDTO>(userobj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserDTO>> GetUsersByUserType(int usertypeId)
        {
            try
            {
                var users = await _context.Users.Where(x => x.UserTypeId == usertypeId).ToListAsync();
                return _mapper.Map<List<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users by type {UserTypeId}", usertypeId);
                throw;
            }
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var objs = await _context.Users.ToListAsync();
                return _mapper.Map<List<UserDTO>>(objs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            try
            {
                var userObj = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
                return _mapper.Map<UserDTO>(userObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                throw;
            }
        }


        // --- NEW METHODS IMPLEMENTATION START ---
        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        // --- NEW METHODS IMPLEMENTATION END ---
    }
}