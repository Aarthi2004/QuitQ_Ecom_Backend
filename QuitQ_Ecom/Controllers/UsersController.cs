using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Interfaces;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data provided.", errors = ModelState });

            try
            {
                var userCreated = await _userService.RegisterUser(user);
                return Ok(new { success = true, data = userCreated });
            }
            catch (AddUserException ex)
            {
                _logger.LogError(ex, "Registration failed with AddUserException.");
                // Return a specific error code for a controlled exception
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration.");
                // Return a generic 500 for truly unhandled exceptions
                return StatusCode(500, new { success = false, message = "An unexpected error occurred on the server." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("usertype/{usertypeId:int}")]
        public async Task<IActionResult> GetUserByUserType(int usertypeId)
        {
            var users = await _userService.GetUsersByUserType(usertypeId);
            return Ok(users);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpDelete("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserById(int userId)
        {
            await _userService.DeleteUserByIdAsync(userId);
            return Ok("User deleted successfully.");
        }
    }
}
