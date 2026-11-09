using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly PasswordHasher<AppUser> _hasher = new();

    public AuthController(RaceDayDbContext db) => _db = db;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict("An account with that email already exists.");

        var roleName = string.Equals(request.Role, "Organiser", StringComparison.OrdinalIgnoreCase)
            ? "Organiser" : "Participant";
        var role = await _db.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            role = new Role { Name = roleName };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
        }

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            RoleId = role.RoleId
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Created("api/auth/register", null);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users.Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        HttpContext.Session.SetInt32("UserId", user.AppUserId);
        HttpContext.Session.SetString("Role", user.Role.Name);
        return Ok(new AuthResponse
        {
            UserId = user.AppUserId,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role.Name
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok();
    }
}