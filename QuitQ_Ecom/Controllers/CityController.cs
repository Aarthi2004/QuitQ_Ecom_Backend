using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Exceptions;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ILogger<CityController> _logger;

        public CityController(ICityService cityService, ILogger<CityController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                var cities = await _cityService.GetAllCities();
                return Ok(cities);
            }
            catch (GetAllCitiesException ex)
            {
                _logger.LogError(ex, "Error occurred while getting all cities: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetCityById(int cityId)
        {
            try
            {
                var city = await _cityService.GetCityById(cityId);
                return Ok(city);
            }
            catch (CityNotFoundException)
            {
                return NotFound("City not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting city by ID {CityId}: {Message}", cityId, ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost("")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCity([FromBody] CityDTO cityDTO)
        {
            try
            {
                var addedCity = await _cityService.AddCity(cityDTO);
                return CreatedAtAction(nameof(GetCityById), new { cityId = addedCity.CityId }, addedCity);
            }
            catch (AddCityException ex)
            {
                _logger.LogError(ex, "Error occurred while adding city: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{cityId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCityState(int cityId, [FromQuery] int stateId)
        {
            try
            {
                var updatedCity = await _cityService.UpdateCityState(cityId, stateId);
                return Ok(updatedCity);
            }
            catch (CityNotFoundException)
            {
                return NotFound("City not found");
            }
            catch (StateNotFoundException)
            {
                return NotFound("State not found");
            }
            catch (UpdateCityStateException ex)
            {
                _logger.LogError(ex, "Error occurred while updating city state: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{cityId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCity(int cityId)
        {
            try
            {
                await _cityService.DeleteCity(cityId);
                return Ok("City deleted successfully");
            }
            catch (CityNotFoundException)
            {
                return NotFound("City not found");
            }
            catch (DeleteCityException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting city: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}