using QuitQ_Ecom.DTOs;

namespace QuitQ_Ecom.Services
{
    public interface IBrandService
    {
        Task<List<BrandDTO>> GetAllBrands();
        Task<BrandDTO> GetBrandById(int brandId);
        Task<BrandDTO> AddBrand(BrandDTO brandDTO);
        Task<BrandDTO> UpdateBrand(int brandId, BrandDTO brandDTO);
        Task<bool> DeleteBrand(int brandId);
    }
}