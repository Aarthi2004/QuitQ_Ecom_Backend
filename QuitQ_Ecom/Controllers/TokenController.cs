using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<TokenController> _logger;

        public TokenController(ITokenService tokenService, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login request");

            try
            {
                // The service now returns our complete DTO or null
                var loginResponse = await _tokenService.AuthenticateAsync(login);

                if (loginResponse == null)
                    return Unauthorized("Invalid credentials"); // Use 401 Unauthorized for bad credentials

                // The loginResponse object has the exact shape our React frontend needs.
                // We can return it directly. This resolves the compiler error.
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login process failed for user {Username}", login.Username);
                return StatusCode(500, "An unexpected error occurred during login.");
            }
        }
    }
}