using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAll();
                return Ok(products);
            }
            catch (GetAllProductsException ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetById(id);
                return product == null ? NotFound("Not Found") : Ok(product);
            }
            catch (GetProductByIdException ex)
            {
                _logger.LogError(ex, $"Error getting product with ID {id}");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/products/search?query=...
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            try
            {
                var products = await _productService.Search(query);
                return Ok(products);
            }
            catch (SearchProductException ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/products/bysubcategory/1
        [HttpGet("bysubcategory/{subcategoryId:int}")]
        public async Task<IActionResult> GetProductsBySubcategoryID(int subcategoryId)
        {
            try
            {
                var products = await _productService.GetBySubCategory(subcategoryId);
                return Ok(products);
            }
            catch (GetProductsBySubCategoryException ex)
            {
                _logger.LogError(ex, $"Error getting products by subcategory ID {subcategoryId}");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/products/bystore/1
        [HttpGet("bystore/{storeId:int}")]
        public async Task<IActionResult> GetAllProductsByStoreId(int storeId)
        {
            try
            {
                var products = await _productService.GetAllByStore(storeId);
                return Ok(products);
            }
            catch (GetAllProductsByStoreIdException ex)
            {
                _logger.LogError(ex, $"Error getting all products by store ID {storeId}");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] ProductDTO productDto)
        {
            try
            {
                // Correctly call the service with just the DTO
                var addedProduct = await _productService.AddProduct(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = addedProduct.ProductId }, addedProduct);
            }
            catch (AddProductException ex)
            {
                _logger.LogError(ex, "Error adding product");
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a product.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDTO productDto, [FromForm] List<ProductDetailDTO> details)
        {
            try
            {
                // Set the DTO's ID from the URL to ensure it's correct
                productDto.ProductId = id;
                var updated = await _productService.Update(id, productDto, details);
                return Ok(updated);
            }
            catch (UpdateProductException ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {id}");
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating a product.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProductByID(int productId)
        {
            try
            {
                var deleted = await _productService.Delete(productId);
                return deleted
                    ? Ok("product deleted Successfully")
                    : NotFound("Product not found");
            }
            catch (DeleteProductException ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {productId}");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/products/filter
        [HttpPost("filter")]
        public async Task<IActionResult> FilterProducts([FromBody] ProductFilterDTO filter)
        {
            try
            {
                var products = await _productService.Filter(filter);
                return Ok(products);
            }
            catch (FilterProductsException ex)
            {
                _logger.LogError(ex, "Error filtering products");
                return StatusCode(500, ex.Message);
            }
        }
    }
}