using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface ICityService
    {
        Task<CityDTO> GetCityById(int cityId);
        Task<CityDTO> UpdateCityState(int cityId, int stateId);
        Task<List<CityDTO>> GetAllCities();
        Task<CityDTO> AddCity(CityDTO cityDTO);
        Task<bool> DeleteCity(int cityId);
    }
}