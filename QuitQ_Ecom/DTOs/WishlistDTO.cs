using System.ComponentModel.DataAnnotations;

namespace QuitQ_Ecom.DTOs
{
    public class WishListDTO
    {
        public int WishListId { get; set; }
        [Required(ErrorMessage = "User ID is required.")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int? ProductId { get; set; }
        
        // Add this line to include the product details
        public ProductDTO? Product { get; set; }
    }
}