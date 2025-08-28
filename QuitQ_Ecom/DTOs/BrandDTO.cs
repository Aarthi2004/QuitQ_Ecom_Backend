using Microsoft.AspNetCore.Http;

namespace QuitQ_Ecom.DTOs
{
    public class BrandDTO
    {
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public IFormFile? BrandLogoImg { get; set; } // Nullable to handle updates without a new image
        public string? BrandLogo { get; set; }
    }
}