using QuitQ_Ecom.Models;

namespace QuitQ_Ecom.Repository
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllBrands();
        Task<Brand> GetBrandById(int brandId);
        Task<Brand> AddBrand(Brand brand);
        Task<Brand> UpdateBrand(Brand brand);
        Task<bool> DeleteBrand(int brandId);
    }
}