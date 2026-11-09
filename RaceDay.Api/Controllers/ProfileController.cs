using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Profiles;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ProfileController(RaceDayDbContext db) => _db = db;
    private int? UserId => HttpContext.Session.GetInt32("UserId");
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        if (UserId == null) return Unauthorized();
        var user = await _db.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.AppUserId == UserId);
        return user == null ? NotFound() : Ok(new ProfileDto { UserId = user.AppUserId, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Role = user.Role.Name, ProfileImageUrl = user.ProfileImageUrl });
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (UserId == null) return Unauthorized();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.AppUserId == UserId);
        if (user == null) return NotFound();
        user.FirstName = request.FirstName; user.LastName = request.LastName; user.Email = request.Email;
        await _db.SaveChangesAsync(); return NoContent();
    }

    [HttpPost("me/image")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromServices] BlobStorageService blobStorage)
    {
        if (UserId == null) return Unauthorized();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.AppUserId == UserId);
        if (user == null) return NotFound();
        user.ProfileImageUrl = await blobStorage.UploadAsync(file, "profile-images");
        await _db.SaveChangesAsync();
        return Ok(new { imageUrl = user.ProfileImageUrl });
    }
}
