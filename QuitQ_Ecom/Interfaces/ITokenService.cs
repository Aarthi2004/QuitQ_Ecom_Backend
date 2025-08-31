using QuitQ_Ecom.DTOs; // <-- Add this using statement

namespace QuitQ_Ecom.Interfaces
{
    public interface ITokenService
    {
        // Change the return type from Task<string> to the new DTO
        Task<LoginResponseDTO> AuthenticateAsync(LoginDTO login);
    }
}