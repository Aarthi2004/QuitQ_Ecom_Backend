using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QuitQ_Ecom.DTOs
{
    public class ShipperDTO
    {
        [Required]
        public int ShipperId { get; set; }
        [JsonPropertyName("OTP")]
        [StringLength(100)]
        public string? ShipperName { get; set; }
        [Range(1, int.MaxValue)]
        public int? OrderId { get; set; }
    }
}
