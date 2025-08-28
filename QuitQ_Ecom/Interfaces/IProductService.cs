// In QuitQ_Ecom.Interfaces/IProductService.cs
using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IProductService
    {
        // Change this line:
        Task<ProductDTO> AddProduct(ProductDTO dto);

        // ... all other methods remain the same
        Task<List<ProductDTO>> CheckQuantity(List<CartDTO> cartItems);
        Task<bool> Delete(int id);
        Task<ProductDTO> GetById(int id);
        Task<List<ProductDTO>> GetBySubCategory(int subCatId);
        Task<ProductDTO> Update(int id, ProductDTO dto, List<ProductDetailDTO> details);
        Task<bool> UpdateQuantities(List<CartDTO> cartItems);
        Task<List<ProductDTO>> Search(string query);
        Task<List<ProductDTO>> GetAll();
        Task<List<ProductDTO>> GetAllByStore(int storeId);
        Task<List<ProductDTO>> Filter(ProductFilterDTO filterDto);
    }
}