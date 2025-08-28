using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models;

namespace QuitQ_Ecom.Interfaces
{
    public interface IStore
    {
        Task<Store> AddStore(Store store);
        Task<List<Store>> GetAllStores();
        Task<Store> GetStoreById(int storeId);
        Task<Store> UpdateStore(Store store);
        Task<bool> DeleteStore(int storeId);
        Task<List<Product>> GetProductsByStore(int storeId);
        Task<List<Store>> GetAllStoresOfUserByUserId(int userId);
    }
}