using AutoMapper;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace QuitQ_Ecom.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IStateRepository _stateRepository; // Inject state repository to validate stateId
        private readonly IMapper _mapper;
        private readonly ILogger<CityService> _logger;

        public CityService(ICityRepository cityRepository, IStateRepository stateRepository, IMapper mapper, ILogger<CityService> logger)
        {
            _cityRepository = cityRepository;
            _stateRepository = stateRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CityDTO> GetCityById(int cityId)
        {
            try
            {
                var city = await _cityRepository.GetCityById(cityId);
                if (city == null)
                {
                    throw new CityNotFoundException($"City with ID {cityId} not found.");
                }
                return _mapper.Map<CityDTO>(city);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCityById for ID {CityId}.", cityId);
                throw new CityNotFoundException("Failed to get city by ID.", ex);
            }
        }

        public async Task<CityDTO> UpdateCityState(int cityId, int stateId)
        {
            try
            {
                var city = await _cityRepository.GetCityById(cityId);
                if (city == null)
                {
                    throw new CityNotFoundException($"City with ID {cityId} not found.");
                }

                var state = await _stateRepository.GetStateById(stateId);
                if (state == null)
                {
                    throw new StateNotFoundException($"State with ID {stateId} not found.");
                }

                city.StateId = stateId;
                var updatedCity = await _cityRepository.UpdateCityState(cityId, stateId);
                return _mapper.Map<CityDTO>(updatedCity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCityState for City ID {CityId} and State ID {StateId}.", cityId, stateId);
                throw new UpdateCityStateException("Failed to update city state.", ex);
            }
        }

        public async Task<List<CityDTO>> GetAllCities()
        {
            try
            {
                var cities = await _cityRepository.GetAllCities();
                return _mapper.Map<List<CityDTO>>(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCities.");
                throw new GetAllCitiesException("Failed to get all cities.", ex);
            }
        }

        public async Task<CityDTO> AddCity(CityDTO cityDTO)
        {
            try
            {
                var city = _mapper.Map<City>(cityDTO);
                var addedCity = await _cityRepository.AddCity(city);
                return _mapper.Map<CityDTO>(addedCity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddCity.");
                throw new AddCityException("Failed to add city.", ex);
            }
        }

        public async Task<bool> DeleteCity(int cityId)
        {
            try
            {
                var deleted = await _cityRepository.DeleteCity(cityId);
                if (!deleted)
                {
                    throw new CityNotFoundException($"City with ID {cityId} not found.");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCity for ID {CityId}.", cityId);
                throw new DeleteCityException("Failed to delete city.", ex);
            }
        }
    }
}