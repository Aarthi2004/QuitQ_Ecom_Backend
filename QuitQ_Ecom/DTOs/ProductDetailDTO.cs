namespace QuitQ_Ecom.DTOs
{
    public class ProductDetailDTO
    {
        public int ProductDetailId { get; set; } // same as model
        public int? ProductId { get; set; }
        public string Attribute { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}