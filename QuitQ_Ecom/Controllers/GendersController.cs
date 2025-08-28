using Microsoft.AspNetCore.Mvc;
using QuitQ_Ecom.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/genders")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly IGenderService _genderService;
        private readonly ILogger<GendersController> _logger;

        public GendersController(IGenderService genderService, ILogger<GendersController> logger)
        {
            _genderService = genderService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllGenders()
        {
            try
            {
                var genders = await _genderService.GetAllGenders();
                return Ok(genders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving genders: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{genderId:int}")]
        public async Task<IActionResult> GetGenderById(int genderId)
        {
            try
            {
                var gender = await _genderService.GetGenderById(genderId);
                if (gender == null)
                    return NotFound($"Gender with ID {genderId} not found.");
                return Ok(gender);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddGender([FromBody] GenderDTO genderDTO)
        {
            try
            {
                var addedGender = await _genderService.AddGender(genderDTO);
                return CreatedAtAction(nameof(GetGenderById), new { genderId = addedGender.GenderId }, addedGender);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{genderId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateGender(int genderId, [FromBody] GenderDTO genderDTO)
        {
            try
            {
                if (genderId != genderDTO.GenderId)
                    return BadRequest("Gender ID mismatch.");

                var updatedGender = await _genderService.UpdateGender(genderDTO);
                return Ok(updatedGender);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{genderId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteGender(int genderId)
        {
            try
            {
                var deleted = await _genderService.DeleteGender(genderId);
                if (!deleted)
                    return NotFound($"Gender with ID {genderId} not found.");
                return Ok($"Gender with ID {genderId} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
