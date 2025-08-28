namespace QuitQ_Ecom.DTOs
{
    public class ImageDTO
    {
        public int ImageId { get; set; }
        public int? ProductId { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public string StoredImage { get; set; } = string.Empty;
    }
}
