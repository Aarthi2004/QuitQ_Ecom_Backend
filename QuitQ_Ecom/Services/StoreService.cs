using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStore _storeRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<StoreService> _logger;
        private readonly IWebHostEnvironment _env;

        public StoreService(IStore storeRepo, IMapper mapper, ILogger<StoreService> logger, IWebHostEnvironment env)
        {
            _storeRepo = storeRepo;
            _mapper = mapper;
            _logger = logger;
            _env = env;
        }

        private async Task<string> SaveStoreImage(StoreDTO storeDTO)
        {
            if (storeDTO.StoreImageFile == null || storeDTO.StoreImageFile.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + storeDTO.StoreImageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await storeDTO.StoreImageFile.CopyToAsync(stream);
            }

            return "/Uploads/" + uniqueFileName;
        }

        public async Task<StoreDTO> AddStore(StoreDTO storeDTO)
        {
            try
            {
                storeDTO.StoreLogo = await SaveStoreImage(storeDTO);
                var store = _mapper.Map<Store>(storeDTO);
                var addedStore = await _storeRepo.AddStore(store);
                return _mapper.Map<StoreDTO>(addedStore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddStore service method.");
                throw;
            }
        }

        public async Task<bool> DeleteStore(int storeId)
        {
            try
            {
                return await _storeRepo.DeleteStore(storeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting store with ID {storeId}.");
                throw;
            }
        }

        public async Task<List<StoreDTO>> GetAllStores()
        {
            try
            {
                var stores = await _storeRepo.GetAllStores();
                return _mapper.Map<List<StoreDTO>>(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all stores.");
                throw;
            }
        }

        public async Task<List<StoreDTO>> GetAllStoresOfUserByUserId(int userId)
        {
            try
            {
                var stores = await _storeRepo.GetAllStoresOfUserByUserId(userId);
                if (stores == null || stores.Count == 0)
                {
                    return null;
                }
                return _mapper.Map<List<StoreDTO>>(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stores for user ID {userId}.");
                throw;
            }
        }

        public async Task<StoreDTO> GetStoreById(int storeId)
        {
            try
            {
                var store = await _storeRepo.GetStoreById(storeId);
                return _mapper.Map<StoreDTO>(store);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving store with ID {storeId}.");
                throw;
            }
        }

        public async Task<StoreDTO> UpdateStore(int storeId, StoreDTO storeDTO)
        {
            try
            {
                // 1. Get the existing store from the database. This is correct.
                var existingStore = await _storeRepo.GetStoreById(storeId);
                if (existingStore == null)
                {
                    return null; // Store not found
                }

                // 2. Manually update the properties from the DTO.
                // This is safer than using AutoMapper for partial updates from a form.
                existingStore.StoreName = storeDTO.StoreName;
                existingStore.Description = storeDTO.Description;
                existingStore.StoreFullAddress = storeDTO.StoreFullAddress;
                existingStore.CityId = storeDTO.CityId;
                existingStore.ContactNumber = storeDTO.ContactNumber;
                existingStore.SellerId = storeDTO.SellerId; // Ensure sellerId from the token is used.

                // 3. Handle the image logic separately and correctly.
                if (storeDTO.StoreImageFile != null && storeDTO.StoreImageFile.Length > 0)
                {
                    // If a new image is provided, save it and update the path.
                    existingStore.StoreLogo = await SaveStoreImage(storeDTO);
                }
                // If no new image is provided, we don't need an 'else' block
                // because we are not overwriting the existingStore.StoreLogo.
                // The original value remains, which is the desired behavior.

                // 4. Call the repository to save the updated entity.
                var updatedStore = await _storeRepo.UpdateStore(existingStore);

                if (updatedStore == null)
                {
                    _logger.LogError($"UpdateStore failed in repository for store with ID {storeId}.");
                    return null;
                }

                // 5. Return the updated DTO.
                return _mapper.Map<StoreDTO>(updatedStore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating store with ID {storeId}.");
                throw;
            }
        }
    }
}