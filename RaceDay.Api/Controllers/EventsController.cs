using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    private int? UserId => HttpContext.Session.GetInt32("UserId");
    private bool IsOrganiser => string.Equals(HttpContext.Session.GetString("Role"), "Organiser", StringComparison.OrdinalIgnoreCase);

    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        return Ok(await _db.Events.AsNoTracking().OrderBy(e => e.EventDate).Select(ToDto).ToListAsync());
    }

    [HttpPost("{id}/banner")]
    public async Task<IActionResult> UploadBanner(int id, IFormFile file, [FromServices] BlobStorageService blobStorage)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        var item = await _db.Events.SingleOrDefaultAsync(e => e.EventId == id && e.OrganiserId == UserId);
        if (item == null) return NotFound();
        item.BannerImageUrl = await blobStorage.UploadAsync(file, "event-banners");
        await _db.SaveChangesAsync();
        return Ok(new { imageUrl = item.BannerImageUrl });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var item = await _db.Events.AsNoTracking().Where(e => e.EventId == id).Select(ToDto).SingleOrDefaultAsync();
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyEvents()
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        return Ok(await _db.Events.AsNoTracking().Where(e => e.OrganiserId == UserId).OrderBy(e => e.EventDate).Select(ToDto).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        var item = new Event
        {
            Name = request.Name, Description = request.Description, EventDate = request.EventDate,
            Location = request.Location, DistanceKm = request.DistanceKm, EventType = request.EventType,
            OrganiserId = UserId.Value
        };
        _db.Events.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEvent), new { id = item.EventId }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        var item = await _db.Events.SingleOrDefaultAsync(e => e.EventId == id && e.OrganiserId == UserId);
        if (item == null) return NotFound();
        item.Name = request.Name; item.Description = request.Description; item.EventDate = request.EventDate;
        item.Location = request.Location; item.DistanceKm = request.DistanceKm; item.EventType = request.EventType;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        if (!IsOrganiser || UserId == null) return Unauthorized();
        var item = await _db.Events.SingleOrDefaultAsync(e => e.EventId == id && e.OrganiserId == UserId);
        if (item == null) return NotFound();
        _db.Events.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static readonly System.Linq.Expressions.Expression<Func<Event, EventDto>> ToDto = e => new EventDto
    {
        EventId = e.EventId, Name = e.Name, Description = e.Description, EventDate = e.EventDate,
        Location = e.Location, DistanceKm = e.DistanceKm, EventType = e.EventType, BannerImageUrl = e.BannerImageUrl
    };
}