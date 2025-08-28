using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    // The service interface that the controller will depend on
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task<List<SubCategoryDTO>> GetSubCategoriesByCategoryId(int categoryId);
        Task<CategoryDTO> AddCategory(CategoryDTO categoryDTO);
        Task<CategoryDTO> GetCategoryById(int categoryId);
        Task<CategoryDTO> UpdateCategory(CategoryDTO categoryDTO);
        Task<bool> DeleteCategory(int categoryId);
        Task<List<ProductDTO>> GetProductsByCategory(int categoryId);
    }
}