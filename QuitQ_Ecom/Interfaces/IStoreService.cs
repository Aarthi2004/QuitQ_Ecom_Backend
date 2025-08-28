using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IStoreService
    {
        Task<StoreDTO> AddStore(StoreDTO storeDTO);
        Task<List<StoreDTO>> GetAllStores();
        Task<StoreDTO> GetStoreById(int storeId);
        Task<StoreDTO> UpdateStore(int storeId, StoreDTO storeDTO);
        Task<bool> DeleteStore(int storeId);
        Task<List<StoreDTO>> GetAllStoresOfUserByUserId(int userId);
    }
}