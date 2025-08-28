using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Repository.Exceptions;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/states")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly ILogger<StateController> _logger;

        public StateController(IStateService stateService, ILogger<StateController> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var states = await _stateService.GetAllStates();
                return Ok(states);
            }
            catch (GetAllStatesException ex)
            {
                _logger.LogError(ex, $"An error occurred while getting all states: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{stateId}")]
        public async Task<IActionResult> GetStateById(int stateId)
        {
            try
            {
                var state = await _stateService.GetStateById(stateId);
                return Ok(state);
            }
            catch (StateNotFoundException)
            {
                return NotFound("State not found");
            }
            catch (GetStateByIdException ex)
            {
                _logger.LogError(ex, $"An error occurred while getting state by ID: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddState([FromBody] StateDTO stateDTO)
        {
            try
            {
                var addedState = await _stateService.AddState(stateDTO);
                return CreatedAtAction(nameof(GetStateById), new { stateId = addedState.StateId }, addedState);
            }
            catch (NullStateDtoException)
            {
                return BadRequest("StateDTO cannot be null.");
            }
            catch (AddStateException ex)
            {
                _logger.LogError(ex, $"An error occurred while adding state: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{stateId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateState(int stateId, [FromBody] StateDTO stateDTO)
        {
            try
            {
                var updatedState = await _stateService.UpdateState(stateId, stateDTO);
                return Ok(updatedState);
            }
            catch (StateNotFoundException)
            {
                return NotFound("State not found");
            }
            catch (UpdateStateException ex)
            {
                _logger.LogError(ex, $"An error occurred while updating state: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{stateId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteState(int stateId)
        {
            try
            {
                await _stateService.DeleteState(stateId);
                return Ok("State deleted successfully");
            }
            catch (StateNotFoundException)
            {
                return NotFound("State not found");
            }
            catch (DeleteStateException ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting state: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("cities/{stateId}")]
        public async Task<IActionResult> GetAllCitiesByStateId(int stateId)
        {
            try
            {
                var cities = await _stateService.GetCitiesByStateId(stateId);
                return Ok(cities);
            }
            catch (GetCitiesByStateIdException ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching cities of the state: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}