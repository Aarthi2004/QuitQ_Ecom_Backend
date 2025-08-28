using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class StoreRepositoryImpl : IStore
    {
        private readonly QuitQEcomContext _context;
        private readonly ILogger<StoreRepositoryImpl> _logger;

        public StoreRepositoryImpl(QuitQEcomContext quitQEcomContext, ILogger<StoreRepositoryImpl> logger)
        {
            _context = quitQEcomContext;
            _logger = logger;
        }

        public async Task<Store> AddStore(Store store)
        {
            try
            {
                await _context.Stores.AddAsync(store);
                await _context.SaveChangesAsync();
                return store;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the store.");
                throw new Exception("Database error occurred while adding the store.");
            }
        }

        public async Task<bool> DeleteStore(int storeId)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                {
                    return false;
                }

                var productsToRemove = _context.Products.Where(p => p.StoreId == storeId);
                _context.Products.RemoveRange(productsToRemove);
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the store with ID {storeId}.");
                throw new Exception($"Database error occurred while deleting the store.");
            }
        }

        public async Task<List<Store>> GetAllStores()
        {
            try
            {
                return await _context.Stores.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all stores.");
                throw new Exception("Database error occurred while retrieving all stores.");
            }
        }

        public async Task<List<Store>> GetAllStoresOfUserByUserId(int userId)
        {
            try
            {
                return await _context.Stores.Where(x => x.SellerId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving stores for user ID {userId}.");
                throw new Exception("Database error occurred while retrieving user stores.");
            }
        }

        public async Task<List<Product>> GetProductsByStore(int storeId)
        {
            try
            {
                return await _context.Products.Where(p => p.StoreId == storeId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving products for store ID {storeId}.");
                throw new Exception("Database error occurred while retrieving products for store.");
            }
        }

        public async Task<Store> GetStoreById(int storeId)
        {
            try
            {
                return await _context.Stores.FindAsync(storeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the store with ID {storeId}.");
                throw new Exception($"Database error occurred while retrieving the store.");
            }
        }

        public async Task<Store> UpdateStore(Store store)
        {
            try
            {
                _context.Stores.Update(store);
                await _context.SaveChangesAsync();
                return store;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the store with ID {store.StoreId}.");
                throw new Exception("Database error occurred while updating the store.");
            }
        }
    }
}