using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class ProductRepositoryImpl : IProduct
    {
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepositoryImpl> _logger;

        public ProductRepositoryImpl(QuitQEcomContext context, IMapper mapper, ILogger<ProductRepositoryImpl> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDTO> AddNewProduct(ProductDTO productDTO, List<ImageDTO> imagesDTOs)
        {
            try
            {
                if (productDTO.Quantity == 0)
                {
                    productDTO.ProductStatusId = 2; // Assuming 2 means 'Out of Stock'
                }

                var product = _mapper.Map<Product>(productDTO);

                if (productDTO.ProductDetails != null && productDTO.ProductDetails.Any())
                {
                    product.ProductDetails = _mapper.Map<List<ProductDetail>>(productDTO.ProductDetails);
                }

                if (imagesDTOs != null && imagesDTOs.Any())
                {
                    product.Images = _mapper.Map<List<Image>>(imagesDTOs);
                }

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                var newProductWithIncludes = await _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

                return _mapper.Map<ProductDTO>(newProductWithIncludes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding new product: {ex.Message}");
                throw new AddProductException($"Error adding new product: {ex.Message}");
            }
        }

        public async Task<List<ProductDTO>> CheckQuantityOfProducts(List<CartDTO> cartItems)
        {
            try
            {
                // Refactored to avoid N+1 queries.
                // Fetch all required products in a single database call.
                var productIds = cartItems.Select(item => item.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToListAsync();

                var outOfStockProducts = new List<Product>();

                foreach (var cartItem in cartItems)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                    if (product != null && product.Quantity < cartItem.Quantity)
                    {
                        outOfStockProducts.Add(product);
                    }
                }

                return _mapper.Map<List<ProductDTO>>(outOfStockProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking quantity of products: {ex.Message}");
                throw new CheckProductQuantityException($"Error checking quantity of products: {ex.Message}");
            }
        }

        public async Task<bool> DeleteProductByID(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return false;
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product: {ex.Message}");
                throw new DeleteProductException($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<Product> GetProductById(int productId)
        {
            try
            {
                // A repository method should return the raw entity, not a DTO.
                // The service layer handles mapping from entity to DTO.
                return await _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product by id: {ex.Message}");
                throw new GetProductByIdException($"Error getting product by id: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetProductsByIds(List<int> productIds)
        {
            try
            {
                return await _context.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products by Ids: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ProductDTO>> GetProductsBySubCategory(int subCategoryId)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .Where(x => x.SubCategoryId == subCategoryId)
                    .ToListAsync();
                return _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetProductsBySubCategory: {ex.Message}");
                throw new GetProductsBySubCategoryException($"Error in GetProductsBySubCategory: {ex.Message}");
            }
        }

        public async Task<ProductDTO> UpdateProduct(int productId, ProductDTO formData, List<ProductDetailDTO> listproductdetaildtos)
        {
            try
            {
                var existingProduct = await _context.Products
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (existingProduct == null)
                    throw new InvalidOperationException("Product not found");

                _mapper.Map(formData, existingProduct);

                if (listproductdetaildtos != null && listproductdetaildtos.Any())
                {
                    _context.ProductDetails.RemoveRange(existingProduct.ProductDetails);
                    existingProduct.ProductDetails = _mapper.Map<List<ProductDetail>>(listproductdetaildtos);
                }

                if (formData.Images != null && formData.Images.Any())
                {
                    _context.Images.RemoveRange(existingProduct.Images);
                    existingProduct.Images = _mapper.Map<List<Image>>(formData.Images);
                }

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();

                var updatedProductWithIncludes = await _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == existingProduct.ProductId);

                return _mapper.Map<ProductDTO>(updatedProductWithIncludes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product: {ex.Message}");
                throw new UpdateProductException($"Error updating product: {ex.Message}");
            }
        }

        public async Task<bool> UpdateQuantitiesOfProducts(List<CartDTO> cartItems)
        {
            try
            {
                // Refactored to avoid N+1 queries.
                // Fetch all products that need updating in a single query.
                var productIdsToUpdate = cartItems.Select(item => item.ProductId).ToList();
                var productsToUpdate = await _context.Products
                    .Where(p => productIdsToUpdate.Contains(p.ProductId))
                    .ToListAsync();

                foreach (var product in productsToUpdate)
                {
                    var cartItem = cartItems.FirstOrDefault(item => item.ProductId == product.ProductId);
                    if (cartItem != null)
                    {
                        product.Quantity -= cartItem.Quantity;
                        if (product.Quantity <= 0)
                            product.ProductStatusId = 2; // Assuming 2 means 'Out of Stock'
                        _context.Update(product);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating quantities of products: {ex.Message}");
                throw new UpdateProductQuantityException($"Error updating quantities of products: {ex.Message}");
            }
        }

        public async Task<List<ProductDTO>> SearchProducts(string query)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .Where(p => EF.Functions.Like(p.ProductName, $"%{query}%"))
                    .ToListAsync();
                return _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching products: {ex.Message}");
                throw new SearchProductException($"Error searching products: {ex.Message}");
            }
        }

        public async Task<List<ProductDTO>> GetAllProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.ProductDetails)
                    .Include(p => p.Images)
                    .ToListAsync();
                return _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all products: {ex.Message}");
                throw new GetAllProductsException($"Error getting all products: {ex.Message}");
            }
        }

        public async Task<List<ProductDTO>> GetAllProductsByStoreId(int storeId)
        {
            try
            {
                var products = await _context.Products
                    .Where(x => x.StoreId == storeId)
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(x => x.Images)
                    .Include(x => x.ProductDetails)
                    .ToListAsync();
                return _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all products by storeId: {ex.Message}");
                throw new GetAllProductsByStoreIdException($"Error getting all products by storeId: {ex.Message}");
            }
        }

        public async Task<List<ProductDTO>> FilterProducts(ProductFilterDTO filterDTO)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Store)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.Images)
                    .Include(p => p.ProductDetails)
                    .AsQueryable();

                if (filterDTO.MinPrice.HasValue) query = query.Where(p => p.Price >= filterDTO.MinPrice);
                if (filterDTO.MaxPrice.HasValue) query = query.Where(p => p.Price <= filterDTO.MaxPrice);
                if (filterDTO.BrandId.HasValue) query = query.Where(p => p.BrandId == filterDTO.BrandId);
                if (filterDTO.CategoryId.HasValue) query = query.Where(p => p.CategoryId == filterDTO.CategoryId);
                if (filterDTO.SubCategoryId.HasValue) query = query.Where(p => p.SubCategoryId == filterDTO.SubCategoryId);

                var products = await query.ToListAsync();
                return _mapper.Map<List<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error filtering products: {ex.Message}");
                throw new FilterProductsException($"Error filtering products: {ex.Message}");
            }
        }
    }
}