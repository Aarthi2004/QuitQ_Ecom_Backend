using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface ICityRepository
    {
        Task<City> GetCityById(int cityId);
        Task<City> UpdateCityState(int cityId, int stateId);
        Task<List<City>> GetAllCities();
        Task<City> AddCity(City city);
        Task<bool> DeleteCity(int cityId);
    }
}