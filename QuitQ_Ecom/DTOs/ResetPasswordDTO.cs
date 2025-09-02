using System.ComponentModel.DataAnnotations;

namespace QuitQ_Ecom.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The new password must be at least 6 characters long.")]
        public string NewPassword { get; set; }
    }
}