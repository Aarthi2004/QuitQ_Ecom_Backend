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

namespace QuitQ_Ecom.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration config, QuitQEcomContext context, IMapper mapper, ILogger<TokenService> logger)
        {
            _config = config;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> AuthenticateAsync(LoginDTO login)
        {
            try
            {
                // Hash password using same method as UserService
                var hashedPassword = UserService.HashPassword(login.Password);

                // Find user
                var userEntity = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == login.Username && u.Password == hashedPassword);

                if (userEntity == null)
                    return null;

                // Get role
                var userRole = await _context.UserTypes
                    .Where(x => x.UserTypeId == userEntity.UserTypeId)
                    .Select(x => x.UserType1)
                    .FirstOrDefaultAsync();

                // Convert to UNIX epoch seconds for iat
                var issuedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                // Build claims
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToString(), ClaimValueTypes.Integer64),
                    new Claim("UserId", userEntity.UserId.ToString()),
                    new Claim("UserName", userEntity.Username),
                    new Claim("Email", userEntity.Email),
                    new Claim(ClaimTypes.Role, userRole ?? "user")
                };

                // Generate token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(2),
                    signingCredentials: signIn);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token generation failed");
                throw;
            }
        }
    }
}
