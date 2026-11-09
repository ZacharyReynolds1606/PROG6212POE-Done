using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Categories;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;
    private int? UserId => HttpContext.Session.GetInt32("UserId");
    private bool IsOrganiser => HttpContext.Session.GetString("Role") == "Organiser";
    [HttpGet("events/{eventId}/categories")]
    public async Task<IActionResult> GetCategories(int eventId)
    {
        return Ok(await _db.Categories.AsNoTracking().Where(c => c.EventId == eventId).ToListAsync());
    }

    [HttpPost("events/{eventId}/categories")]
    public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryRequest request)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        if (!await _db.Events.AnyAsync(e => e.EventId == eventId && e.OrganiserId == UserId)) return NotFound();
        var item = new Category { EventId = eventId, Name = request.Name, Description = request.Description };
        _db.Categories.Add(item); await _db.SaveChangesAsync(); return Created($"api/categories/{item.CategoryId}", item);
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        var item = await _db.Categories.Include(c => c.Event).SingleOrDefaultAsync(c => c.CategoryId == id && c.Event.OrganiserId == UserId);
        if (item == null) return NotFound(); item.Name = request.Name; item.Description = request.Description;
        await _db.SaveChangesAsync(); return NoContent();
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        var item = await _db.Categories.Include(c => c.Event).SingleOrDefaultAsync(c => c.CategoryId == id && c.Event.OrganiserId == UserId);
        if (item == null) return NotFound(); _db.Categories.Remove(item); await _db.SaveChangesAsync(); return NoContent();
    }
}
