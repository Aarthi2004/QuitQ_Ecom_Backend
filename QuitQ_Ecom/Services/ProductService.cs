using AutoMapper;
using Microsoft.AspNetCore.Http; // Needed for IFormFile
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class ProductService : IProductService
    {
        private readonly IProduct _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProduct productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDTO> AddProduct(ProductDTO dto)
        {
            var imagesDTOs = new List<ImageDTO>();
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (dto.ProductImageFile != null && dto.ProductImageFile.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(dto.ProductImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.ProductImageFile.CopyToAsync(stream);

                dto.ProductImage = "/Uploads/" + uniqueFileName;
            }

            if (dto.ImageFiles != null && dto.ImageFiles.Count > 0)
            {
                foreach (var file in dto.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await file.CopyToAsync(stream);

                        imagesDTOs.Add(new ImageDTO
                        {
                            ImageName = file.FileName,
                            StoredImage = "/Uploads/" + uniqueFileName
                        });
                    }
                }
            }

            return await _productRepository.AddNewProduct(dto, imagesDTOs);
        }

        public async Task<List<ProductDTO>> CheckQuantity(List<CartDTO> cartItems)
        {
            var products = await _productRepository.CheckQuantityOfProducts(cartItems);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public Task<bool> Delete(int id) => _productRepository.DeleteProductByID(id);

        public async Task<ProductDTO> GetById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                return null;
            }
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<List<ProductDTO>> GetBySubCategory(int subCatId)
        {
            var products = await _productRepository.GetProductsBySubCategory(subCatId);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO> Update(int id, ProductDTO dto, List<ProductDetailDTO> details)
        {
            // The repository now returns an entity.
            var existingProduct = await _productRepository.GetProductById(id);

            // Null check to prevent dereference errors
            if (existingProduct == null)
            {
                return null;
            }

            // Handle the main product image
            if (dto.ProductImageFile != null && dto.ProductImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(dto.ProductImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.ProductImageFile.CopyToAsync(stream);

                dto.ProductImage = "/Uploads/" + uniqueFileName;
            }
            else
            {
                // No new file was uploaded, so keep the old image path
                dto.ProductImage = _mapper.Map<ProductDTO>(existingProduct).ProductImage;
            }

            // Handle gallery images
            if (dto.ImageFiles != null && dto.ImageFiles.Count > 0)
            {
                dto.Images = new List<ImageDTO>();
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

                foreach (var file in dto.ImageFiles)
                {
                    var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    dto.Images.Add(new ImageDTO
                    {
                        ImageName = file.FileName,
                        StoredImage = "/Uploads/" + uniqueFileName
                    });
                }
            }
            else
            {
                // No new gallery images were uploaded, so keep the old ones
                dto.Images = _mapper.Map<List<ImageDTO>>(existingProduct.Images);
            }

            return await _productRepository.UpdateProduct(id, dto, details);
        }

        public Task<bool> UpdateQuantities(List<CartDTO> cartItems) => _productRepository.UpdateQuantitiesOfProducts(cartItems);

        public async Task<List<ProductDTO>> Search(string query)
        {
            var products = await _productRepository.SearchProducts(query);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<List<ProductDTO>> GetAll()
        {
            var products = await _productRepository.GetAllProducts();
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<List<ProductDTO>> GetAllByStore(int storeId)
        {
            var products = await _productRepository.GetAllProductsByStoreId(storeId);
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<List<ProductDTO>> Filter(ProductFilterDTO filterDto)
        {
            var products = await _productRepository.FilterProducts(filterDto);
            return _mapper.Map<List<ProductDTO>>(products);
        }
    }
}