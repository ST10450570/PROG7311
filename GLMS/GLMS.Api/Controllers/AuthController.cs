using GLMS.Api.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            var adminUser = _config["AdminCredentials:Username"];
            var adminPass = _config["AdminCredentials:Password"];

            if (dto.Username != adminUser || dto.Password != adminPass)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            var key = _config["Jwt:Key"] ?? throw new Exception("JWT Key missing");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiryHours = int.Parse(_config["Jwt:ExpiryHours"] ?? "8");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: credentials);

            return Ok(new LoginResponseDto { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
