using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
// We no longer need BCrypt.Net, so it can be removed.
// using BCrypt.Net; 

namespace QuitQ_Ecom.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly QuitQEcomContext _context;
        private readonly ILogger<TokenService> _logger;
        private readonly IMapper _mapper;


        // Your constructor might have IMapper, so I've added it back.
        public TokenService(IConfiguration config, QuitQEcomContext context, IMapper mapper, ILogger<TokenService> logger)
        {
            _config = config;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> AuthenticateAsync(LoginDTO login)
        {
            try
            {
                // Find user by their username first.
                var userEntity = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == login.Username);

                if (userEntity == null)
                {
                    return null; // User not found
                }

                // --- CHANGE IS HERE ---
                // Instead of BCrypt, we now use your original password hashing logic.
                // We re-hash the password the user typed in and compare it to what's in the database.
                var hashedPassword = UserService.HashPassword(login.Password); // Hashing the login attempt
                if (userEntity.Password != hashedPassword)
                {
                    return null; // Passwords do not match
                }
                // --- END OF CHANGE ---

                // Get user's role
                var userRole = await _context.UserTypes
                    .Where(x => x.UserTypeId == userEntity.UserTypeId)
                    .Select(x => x.UserType1)
                    .FirstOrDefaultAsync();

                // Build claims for the JWT
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userEntity.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim("username", userEntity.Username),
                    new Claim(ClaimTypes.Role, userRole ?? "Customer")
                };

                // Generate the JWT token string
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(2),
                    signingCredentials: signIn);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

                // Create and return the complete response object
                var loginResponse = new LoginResponseDTO
                {
                    UserId = userEntity.UserId,
                    Username = userEntity.Username,
                    Role = userRole,
                    Token = tokenString
                };

                return loginResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed for user {Username}", login.Username);
                throw;
            }
        }
    }
}