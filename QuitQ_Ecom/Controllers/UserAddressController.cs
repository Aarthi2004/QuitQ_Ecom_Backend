using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/user-addresses")]
    [ApiController]
    [Authorize]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;
        private readonly ILogger<UserAddressController> _logger;

        public UserAddressController(IUserAddressService userAddressService, ILogger<UserAddressController> logger)
        {
            _userAddressService = userAddressService;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddUserAddress([FromBody] UserAddressDTO userAddressDTO)
        {
            try
            {
                var addedUserAddress = await _userAddressService.AddUserAddress(userAddressDTO);
                return CreatedAtAction(nameof(AddUserAddress), new { userAddressId = addedUserAddress.UserAddressId }, addedUserAddress);
            }
            catch (UserAddressAddException ex)
            {
                _logger.LogError(ex, $"Error occurred while adding user address: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{userAddressId}")]
        public async Task<IActionResult> UpdateUserAddress([FromRoute] int userAddressId, [FromBody] UserAddressDTO userAddressDTO)
        {
            try
            {
                var updatedUserAddress = await _userAddressService.UpdateUserAddress(userAddressId, userAddressDTO);
                return Ok(updatedUserAddress);
            }
            catch (UserAddressNotFoundException)
            {
                return NotFound("User Address not found");
            }
            catch (UserAddressUpdateException ex)
            {
                _logger.LogError(ex, $"Error occurred while updating user address: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{userAddressId}")]
        public async Task<IActionResult> DeleteUserAddress(int userAddressId)
        {
            try
            {
                await _userAddressService.DeleteUserAddress(userAddressId);
                return Ok("User Address deleted successfully");
            }
            catch (UserAddressNotFoundException)
            {
                return NotFound("User Address not found");
            }
            catch (UserAddressDeleteException ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting user address: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAddressesByUserId(int userId)
        {
            try
            {
                var userAddresses = await _userAddressService.GetUserAddressesByUserId(userId);
                return Ok(userAddresses);
            }
            catch (UserAddressNotFoundException)
            {
                return NotFound("User addresses not found for this user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while getting user addresses by user ID: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("useractive/{userId}")]
        public async Task<IActionResult> GetUserActiveAddressByUserId(int userId)
        {
            try
            {
                var activeUserAddress = await _userAddressService.GetActiveUserAddressByUserId(userId);
                return Ok(activeUserAddress);
            }
            catch (UserAddressNotFoundException)
            {
                return NotFound("Active user address not found for this user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while getting active user address by user ID: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}