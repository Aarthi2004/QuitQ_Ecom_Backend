using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategory _categoryRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategory categoryRepo, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            return await _categoryRepo.GetAllCategories();
        }

        public async Task<List<SubCategoryDTO>> GetSubCategoriesByCategoryId(int categoryId)
        {
            return await _categoryRepo.GetSubCategoriesByCategoryId(categoryId);
        }

        public async Task<CategoryDTO> AddCategory(CategoryDTO categoryDTO)
        {
            return await _categoryRepo.AddCategory(categoryDTO);
        }

        public async Task<CategoryDTO> GetCategoryById(int categoryId)
        {
            return await _categoryRepo.GetCategoryById(categoryId);
        }

        public async Task<CategoryDTO> UpdateCategory(CategoryDTO categoryDTO)
        {
            return await _categoryRepo.UpdateCategory(categoryDTO);
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            return await _categoryRepo.DeleteCategory(categoryId);
        }

        public async Task<List<ProductDTO>> GetProductsByCategory(int categoryId)
        {
            return await _categoryRepo.GetProductsByCategory(categoryId);
        }
    }
}