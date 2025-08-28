using System;
using System.ComponentModel.DataAnnotations;

namespace QuitQ_Ecom.DTOs
{
    public class PaymentDTO
    {
        [Required]
        public int PaymentId { get; set; }

        [Range(1, int.MaxValue)]
        public int? OrderId { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }
    }
}
