namespace QuitQ_Ecom.DTOs
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}