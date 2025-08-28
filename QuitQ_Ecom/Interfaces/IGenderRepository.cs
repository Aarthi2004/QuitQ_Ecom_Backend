using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom
{
    public interface IGenderRepository
    {
        Task<List<Gender>> GetAllGenders();
        Task<Gender> GetGenderById(int genderId);
        Task<Gender> AddGender(Gender gender);
        Task<Gender> UpdateGender(Gender gender);
        Task<bool> DeleteGender(int genderId);
    }
}
