using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// Note: Ensure your namespace is consistent across all files.
// The namespace in your code is QuitQ_Ecom.Repositories.
namespace QuitQ_Ecom.Repositories
{
    public interface IProduct
    {
        // This is the correct signature for a repository method
        Task<Product> GetProductById(int productId);

        // This method should be moved to your IProductService interface
        // Task<ProductDTO> GetProductById(int productId); // Removed due to conflict

        Task<List<Product>> GetProductsByIds(List<int> productIds);
        Task<ProductDTO> AddNewProduct(ProductDTO productDTO, List<ImageDTO> imagesDTO);
        Task<List<ProductDTO>> CheckQuantityOfProducts(List<CartDTO> cartItems);
        Task<bool> DeleteProductByID(int id);
        Task<List<ProductDTO>> GetProductsBySubCategory(int subCategoryId);
        Task<ProductDTO> UpdateProduct(int productId, ProductDTO formData, List<ProductDetailDTO> productDetails);
        Task<bool> UpdateQuantitiesOfProducts(List<CartDTO> cartItems);
        Task<List<ProductDTO>> SearchProducts(string query);
        Task<List<ProductDTO>> GetAllProducts();
        Task<List<ProductDTO>> GetAllProductsByStoreId(int storeId);
        Task<List<ProductDTO>> FilterProducts(ProductFilterDTO filterDTO);
    }
}