using Microsoft.EntityFrameworkCore;
using QuitQ_Ecom.Context;

using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repository
{
    public class CartRepositoryImpl : ICart
    {
        private readonly QuitQEcomContext _context;

        public CartRepositoryImpl(QuitQEcomContext context)
        {
            _context = context;
        }

        public async Task<List<Cart>> GetUserCartItems(int userId)
        {
            return await _context.Carts
                                 .Where(c => c.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Cart> GetCartItemByProductIdAndUserId(int userId, int productId)
        {
            return await _context.Carts
                                 .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<Cart> AddNewProductToCart(Cart cartItem)
        {
            await _context.Carts.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> IncreaseProductQuantity(int cartItemId)
        {
            var cartItem = await _context.Carts.FindAsync(cartItemId);
            if (cartItem == null) return false;
            cartItem.Quantity++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseProductQuantity(int cartItemId)
        {
            var cartItem = await _context.Carts.FindAsync(cartItemId);
            if (cartItem == null) return false;
            cartItem.Quantity--;
            if (cartItem.Quantity <= 0)
            {
                _context.Carts.Remove(cartItem);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveProductFromCart(int cartItemId)
        {
            var cartItem = await _context.Carts.FindAsync(cartItemId);
            if (cartItem == null) return false;
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalCartCost(int userId)
        {
            var cartItems = await _context.Carts
                                          .Where(c => c.UserId == userId)
                                          .Join(_context.Products,
                                                cart => cart.ProductId,
                                                product => product.ProductId,
                                                (cart, product) => new { cart.Quantity, product.Price }) // Corrected line
                                          .ToListAsync();
            return cartItems.Sum(item => item.Quantity * item.Price); // Corrected line
        }
        public async Task<Cart> GetCartItemById(int cartItemId)
        {
            return await _context.Carts.FindAsync(cartItemId);
        }

        public async Task<bool> UpdateCartItem(Cart cartItem)
        {
            _context.Carts.Update(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCartItemsOfUser(int userId)
        {
            var cartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            if (cartItems.Any())
            {
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}