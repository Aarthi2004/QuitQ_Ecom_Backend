using Microsoft.EntityFrameworkCore;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly QuitQEcomContext _context;

        public CityRepository(QuitQEcomContext context)
        {
            _context = context;
        }

        public async Task<City> GetCityById(int cityId)
        {
            return await _context.Cities.FindAsync(cityId);
        }

        public async Task<City> UpdateCityState(int cityId, int stateId)
        {
            var city = await _context.Cities.FindAsync(cityId);
            var state = await _context.States.FindAsync(stateId);

            if (city == null || state == null)
            {
                return null;
            }

            city.StateId = stateId;
            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return city;
        }

        public async Task<List<City>> GetAllCities()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<City> AddCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city;
        }

        public async Task<bool> DeleteCity(int cityId)
        {
            var city = await _context.Cities.FindAsync(cityId);
            if (city == null)
            {
                return false;
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}