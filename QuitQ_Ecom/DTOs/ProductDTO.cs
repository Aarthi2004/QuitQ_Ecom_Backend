using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace QuitQ_Ecom.DTOs
{
    public class ProductDTO
    {
        public int? ProductId { get; set; }

        [Required, StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        public string ProductImage { get; set; } = string.Empty;

        public int? StoreId { get; set; }
        public int? ProductStatusId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        // Extra fields for form data and display
        [JsonIgnore, NotMapped]
        public IFormFile? ProductImageFile { get; set; }

        [JsonIgnore, NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }

        [JsonIgnore, NotMapped]
        public string? sellerName { get; set; }
        [JsonIgnore, NotMapped]
        public string? BrandName { get; set; }
        [JsonIgnore, NotMapped]
        public string? StoreName { get; set; }
        [JsonIgnore, NotMapped]
        public string? CategoryName { get; set; }
        [JsonIgnore, NotMapped]
        public string? SubCategoryName { get; set; }

        // Collections for product details and images
        public List<ProductDetailDTO>? ProductDetails { get; set; }
        public List<ImageDTO>? Images { get; set; }
    }
}