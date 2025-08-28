using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly QuitQEcomContext _context;

        public WishlistRepository(QuitQEcomContext context)
        {
            _context = context;
        }

        public async Task<WishList> AddToWishList(WishList wishList)
        {
            _context.WishLists.Add(wishList);
            await _context.SaveChangesAsync();
            return wishList;
        }

        public async Task<List<WishList>> GetUserWishList(int userId)
        {
            var userWishList = await _context.WishLists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToListAsync();

            return userWishList;
        }

        public async Task<bool> RemoveFromWishList(int userId, int productId)
        {
            var wishListItem = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if (wishListItem != null)
            {
                _context.WishLists.Remove(wishListItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}