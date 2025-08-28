using Microsoft.EntityFrameworkCore;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly QuitQEcomContext _context;

        public UserAddressRepository(QuitQEcomContext context)
        {
            _context = context;
        }

        public async Task<UserAddress> GetActiveUserAddressByUserId(int userId)
        {
            return await _context.UserAddresses
                                 .FirstOrDefaultAsync(x => x.UserId == userId && x.StatusId == 1);
        }

        public async Task<UserAddress> AddUserAddress(UserAddress userAddress)
        {
            _context.UserAddresses.Add(userAddress);
            await _context.SaveChangesAsync();
            return userAddress;
        }

        public async Task<bool> DeleteUserAddress(int userAddressId)
        {
            var userAddress = await _context.UserAddresses.FindAsync(userAddressId);
            if (userAddress == null)
            {
                return false;
            }

            _context.UserAddresses.Remove(userAddress);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserAddress>> GetUserAddressesByUserId(int userId)
        {
            return await _context.UserAddresses.Where(u => u.UserId == userId).ToListAsync();
        }

        public async Task<UserAddress> UpdateUserAddress(int userAddressId, UserAddress userAddress)
        {
            var existingAddress = await _context.UserAddresses.FindAsync(userAddressId);
            if (existingAddress == null)
            {
                return null;
            }

            // Update properties
            existingAddress.DoorNumber = userAddress.DoorNumber;
            existingAddress.ApartmentName = userAddress.ApartmentName;
            existingAddress.Landmark = userAddress.Landmark;
            existingAddress.Street = userAddress.Street;
            existingAddress.CityId = userAddress.CityId;
            existingAddress.PostalCode = userAddress.PostalCode;
            existingAddress.ContactNumber = userAddress.ContactNumber;
            existingAddress.StatusId = userAddress.StatusId;

            _context.Entry(existingAddress).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return existingAddress;
        }
    }
}