using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions; // Use custom exceptions
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository;
using System;
using System.Collections.Generic;

public class BrandRepositoryImpl : IBrandRepository
{
    private readonly QuitQEcomContext _context;
    private readonly ILogger<BrandRepositoryImpl> _logger;

    public BrandRepositoryImpl(QuitQEcomContext context, ILogger<BrandRepositoryImpl> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Repository methods now focus solely on data access and throw specific exceptions
    public async Task<Brand> AddBrand(Brand brand)
    {
        try
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding brand.");
            throw new AddBrandException("A database error occurred while adding the brand.", ex);
        }
    }

    public async Task<bool> DeleteBrand(int brandId)
    {
        try
        {
            var brand = await _context.Brands.FindAsync(brandId);
            if (brand == null)
            {
                return false;
            }
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting brand with ID {BrandId}.", brandId);
            throw new DeleteBrandException($"A database error occurred while deleting brand with ID {brandId}.", ex);
        }
    }

    public async Task<List<Brand>> GetAllBrands()
    {
        try
        {
            return await _context.Brands.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all brands.");
            throw new GetAllBrandsException("A database error occurred while retrieving all brands.", ex);
        }
    }

    public async Task<Brand> GetBrandById(int brandId)
    {
        try
        {
            return await _context.Brands.FindAsync(brandId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving brand with ID {BrandId}.", brandId);
            throw new GetBrandByIdException($"A database error occurred while retrieving brand with ID {brandId}.", ex);
        }
    }

    public async Task<Brand> UpdateBrand(Brand brand)
    {
        try
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return brand;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating brand with ID {BrandId}.", brand.BrandId);
            throw new UpdateBrandException($"A database error occurred while updating brand with ID {brand.BrandId}.", ex);
        }
    }
}