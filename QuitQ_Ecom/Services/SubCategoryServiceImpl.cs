using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class SubCategoryServiceImpl : ISubCategoryService
    {
        private readonly ISubCategory _subCategoryRepository;
        private readonly ILogger<SubCategoryServiceImpl> _logger;

        public SubCategoryServiceImpl(ISubCategory subCategoryRepository, ILogger<SubCategoryServiceImpl> logger)
        {
            _subCategoryRepository = subCategoryRepository;
            _logger = logger;
        }

        public async Task<SubCategoryDTO> AddSubCategory(SubCategoryDTO subCategoryDTO)
        {
            // Business logic goes here before calling the repository
            // For example, validation or other checks

            return await _subCategoryRepository.AddSubCategory(subCategoryDTO);
        }

        public async Task<bool> DeleteSubCategory(int subCategoryId)
        {
            // Business logic for deletion
            return await _subCategoryRepository.DeleteSubCategory(subCategoryId);
        }

        public async Task<List<SubCategoryDTO>> GetAllSubCategories()
        {
            // Business logic before retrieval
            return await _subCategoryRepository.GetAllSubCategories();
        }

        public async Task<SubCategoryDTO> GetSubCategoryById(int subCategoryId)
        {
            // Business logic
            return await _subCategoryRepository.GetSubCategoryById(subCategoryId);
        }

        public async Task<SubCategoryDTO> UpdateSubCategory(SubCategoryDTO subCategoryDTO)
        {
            // Business logic for update
            return await _subCategoryRepository.UpdateSubCategory(subCategoryDTO);
        }
    }
}