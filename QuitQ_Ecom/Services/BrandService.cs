using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<BrandService> _logger;
        private readonly QuitQEcomContext _context; // For handling dependent records

        public BrandService(IBrandRepository brandRepo, IMapper mapper, ILogger<BrandService> logger, QuitQEcomContext context)
        {
            _brandRepo = brandRepo;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public async Task<BrandDTO> AddBrand(BrandDTO brandDTO)
        {
            try
            {
                // Business Logic: Handle file upload
                if (brandDTO.BrandLogoImg != null && brandDTO.BrandLogoImg.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + brandDTO.BrandLogoImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Brands", uniqueFileName);

                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await brandDTO.BrandLogoImg.CopyToAsync(stream);
                    }
                    brandDTO.BrandLogo = "/Uploads/Brands/" + uniqueFileName;
                }

                var brand = _mapper.Map<Brand>(brandDTO);
                var addedBrand = await _brandRepo.AddBrand(brand);
                return _mapper.Map<BrandDTO>(addedBrand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error while adding brand.");
                throw new AddBrandException("Failed to add brand due to a service error.", ex);
            }
        }

        public async Task<bool> DeleteBrand(int brandId)
        {
            try
            {
                var brand = await _brandRepo.GetBrandById(brandId);
                if (brand == null)
                {
                    return false;
                }

                // Business Logic: Handle dependent records and file deletion
                var dependentProducts = await _context.Products.Where(p => p.BrandId == brandId).ToListAsync();
                if (dependentProducts.Any())
                {
                    _context.Products.RemoveRange(dependentProducts);
                    await _context.SaveChangesAsync();
                }

                // Delete the image file from the server
                if (!string.IsNullOrEmpty(brand.BrandLogo))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", brand.BrandLogo.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                return await _brandRepo.DeleteBrand(brandId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error while deleting brand with ID {BrandId}.", brandId);
                throw new DeleteBrandException($"Failed to delete brand with ID {brandId} due to a service error.", ex);
            }
        }

        public async Task<List<BrandDTO>> GetAllBrands()
        {
            try
            {
                var brands = await _brandRepo.GetAllBrands();
                return _mapper.Map<List<BrandDTO>>(brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error while retrieving all brands.");
                throw new GetAllBrandsException("Failed to retrieve all brands due to a service error.", ex);
            }
        }

        public async Task<BrandDTO> GetBrandById(int brandId)
        {
            try
            {
                var brand = await _brandRepo.GetBrandById(brandId);
                return _mapper.Map<BrandDTO>(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error while retrieving brand with ID {BrandId}.", brandId);
                throw new GetBrandByIdException($"Failed to retrieve brand with ID {brandId} due to a service error.", ex);
            }
        }

        public async Task<BrandDTO> UpdateBrand(int brandId, BrandDTO brandDTO)
        {
            try
            {
                var existingBrand = await _brandRepo.GetBrandById(brandId);
                if (existingBrand == null)
                {
                    return null;
                }

                // Business Logic: Handle file upload for update
                if (brandDTO.BrandLogoImg != null && brandDTO.BrandLogoImg.Length > 0)
                {
                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(existingBrand.BrandLogo))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingBrand.BrandLogo.TrimStart('/'));
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }

                    // Save new file
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + brandDTO.BrandLogoImg.FileName;
                    var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Brands", uniqueFileName);

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await brandDTO.BrandLogoImg.CopyToAsync(stream);
                    }
                    brandDTO.BrandLogo = "/Uploads/Brands/" + uniqueFileName;
                }
                else
                {
                    // Keep existing logo if a new one is not provided
                    brandDTO.BrandLogo = existingBrand.BrandLogo;
                }

                var brand = _mapper.Map<Brand>(brandDTO);
                brand.BrandId = brandId; // Ensure correct ID is used for update

                var updatedBrand = await _brandRepo.UpdateBrand(brand);
                return _mapper.Map<BrandDTO>(updatedBrand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error while updating brand with ID {BrandId}.", brandId);
                throw new UpdateBrandException($"Failed to update brand with ID {brandId} due to a service error.", ex);
            }
        }
    }
}