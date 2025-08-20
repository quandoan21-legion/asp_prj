using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
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

        return Ok(new
        {
            message = "Login successful",
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
}
