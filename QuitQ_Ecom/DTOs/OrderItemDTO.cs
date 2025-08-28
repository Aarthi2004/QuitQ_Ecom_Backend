using System.ComponentModel.DataAnnotations;

namespace QuitQ_Ecom.DTOs
{
    public class OrderItemDTO
    {
        [Required]
        public int OrderItemId { get; set; }
        [Range(1, int.MaxValue)]
        public int? OrderId { get; set; }
        [Range(1, int.MaxValue)]
        public int? ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public ProductDTO Product { get; set; }
    }
}
