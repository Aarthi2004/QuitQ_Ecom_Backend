using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public interface ISubCategoryService
    {
        Task<SubCategoryDTO> AddSubCategory(SubCategoryDTO subCategoryDTO);
        Task<List<SubCategoryDTO>> GetAllSubCategories();
        Task<SubCategoryDTO> GetSubCategoryById(int subCategoryId);
        Task<SubCategoryDTO> UpdateSubCategory(SubCategoryDTO subCategoryDTO);
        Task<bool> DeleteSubCategory(int subCategoryId);
    }
}