using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApi.Data;
using MyApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email || u.Username == user.Username))
        {
            return BadRequest(new { message = "Username or Email already exists" });
        }

        user.PasswordHash = HashPassword(user.PasswordHash);
        user.Phone ??= string.Empty;
        user.FirstName = user.FirstName?.Trim() ?? string.Empty;
        user.LastName = user.LastName?.Trim() ?? string.Empty;
        user.RegistrationDate = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful" });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

        if (user == null)
            return Unauthorized(new { message = "Invalid username/email or password" });

        var hashedInput = HashPassword(request.Password);
        if (user.PasswordHash != hashedInput)
            return Unauthorized(new { message = "Invalid username/email or password" });

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            message = "Login successful",
            token = token.Token,
            expiresAtUtc = token.ExpiresAtUtc,
            user.UserID,
            user.Username,
            user.Email,
            user.IsAdmin
        });
    }

    // Utility: Hash password with SHA256
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    // Utility: Create JWT
    private (string Token, DateTime ExpiresAtUtc) GenerateJwtToken(User user)
    {
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expires);
    }

}