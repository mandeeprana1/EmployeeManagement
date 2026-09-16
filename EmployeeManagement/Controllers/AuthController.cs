using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = dto.UserName.Trim();

            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest(new { message = "Username is required" });
            }

            var adminAlreadyExists = await _context.Users.AnyAsync();

            if (adminAlreadyExists)
            {
                return BadRequest(new
                {
                    message = "An Admin account already exists"
                });
            }

            var user = new AppUser
            {
                UserName = userName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Admin account created successfully"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.UserName == dto.UserName.Trim());

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                role = user.Role,
                expiresInMinutes = _configuration["Jwt:DurationInMinutes"]
            });
        }

        private string GenerateJwtToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(
                        _configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}