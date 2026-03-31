using ClinicManagementSystem.Models;
using ClinicManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;

        public LoginController(ILoginService loginService, IConfiguration configuration)
        {
            _configuration = configuration;
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpGet("{UserName}/{Password}")]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            IActionResult response = Unauthorized();
            User dbUser = await _loginService.ValidateUserService(UserName, Password);

            if (dbUser != null)
            {
                var tokenString = GenerateJSONWebToken(dbUser);
                response = Ok(new
                {
                    UserId = dbUser.UserId,
                    Username = dbUser.Username,
                    FullName = dbUser.FullName,
                    RoleId = dbUser.RoleId,
                    RoleName = dbUser.Role?.RoleName,
                    token = tokenString
                });
            }

            return response;
        }

        private string GenerateJSONWebToken(User dbUser)
        {
            // 1. Security key from appsettings
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // 2. Define Algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dbUser.Username),
                new Claim("RoleId", dbUser.RoleId.ToString()),
                new Claim("UserId", dbUser.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 3. Jwt token payload
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"] ?? _configuration["Jwt:Issuer"], // Fallback to Issuer if Audience is not set
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
