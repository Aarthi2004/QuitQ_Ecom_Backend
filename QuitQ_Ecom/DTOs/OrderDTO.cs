using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuitQ_Ecom.DTOs
{
    public class OrderDTO
    {
        [Required]
        public int OrderId { get; set; }
        [Range(1, int.MaxValue)]
        public int? UserId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; }
        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [JsonIgnore]
        public List<OrderItemDTO>? orderItemListDTOs { get; set; }

        [JsonIgnore]
        public string? Shipper { get; set; }
    }
}
