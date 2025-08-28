using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt; // Add this using statement
using System.Linq; // Add this using statement for LINQ
using System.Security.Claims; // Add this using statement for ClaimTypes
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
                var token = await _tokenService.AuthenticateAsync(login);
                if (token == null)
                    return BadRequest("Invalid credentials");

                // Read the JWT token string to access its claims
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Find the claim that contains the user's role
                // The standard claim type for a role is ClaimTypes.Role
                var userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                return Ok(new
                {
                    token = token,
                    username = login.Username,
                    role = userRole // Add the role to the response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login process failed");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}