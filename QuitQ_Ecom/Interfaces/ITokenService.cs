using QuitQ_Ecom.DTOs;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface ITokenService
    {
        Task<string> AuthenticateAsync(LoginDTO login);
    }
}
