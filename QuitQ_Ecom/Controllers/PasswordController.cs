using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<PasswordController> _logger;

        public PasswordController(IUserService userService, ILogger<PasswordController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.ResetPasswordAsync(resetPasswordDTO);
                return Ok(new { success = true, message = "Password has been reset successfully." });
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while resetting password.");
                return StatusCode(500, new { success = false, message = "An internal server error occurred." });
            }
        }
    }
}