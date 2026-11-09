using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Enrolments;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/enrolments")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;
    private int? UserId => HttpContext.Session.GetInt32("UserId");
    [HttpPost]
    public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentRequest request)
    {
        if (UserId == null) return Unauthorized();
        if (!await _db.Events.AnyAsync(e => e.EventId == request.EventId) || !await _db.Categories.AnyAsync(c => c.CategoryId == request.CategoryId && c.EventId == request.EventId)) return BadRequest("Invalid event or category.");
        if (await _db.Enrolments.AnyAsync(e => e.EventId == request.EventId && e.ParticipantId == UserId)) return Conflict("You are already enrolled.");
        var item = new Enrolment { EventId = request.EventId, CategoryId = request.CategoryId, ParticipantId = UserId.Value, Status = "Pending" };
        _db.Enrolments.Add(item); await _db.SaveChangesAsync(); return Created("api/enrolments", item);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyEnrolments()
    {
        if (UserId == null) return Unauthorized();
        return Ok(await _db.Enrolments.AsNoTracking().Where(e => e.ParticipantId == UserId).Include(e => e.Event).Include(e => e.Category).Select(e => new EnrolmentDto { EnrolmentId = e.EnrolmentId, EventName = e.Event.Name, CategoryName = e.Category.Name, Status = e.Status }).ToListAsync());
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetEventEnrolments(int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return Unauthorized();
        return Ok(await _db.Enrolments.AsNoTracking().Where(e => e.EventId == eventId).Include(e => e.Event).Include(e => e.Category).Select(e => new EnrolmentDto { EnrolmentId = e.EnrolmentId, EventName = e.Event.Name, CategoryName = e.Category.Name, Status = e.Status }).ToListAsync());
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateEnrolmentStatusRequest request)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return Unauthorized();
        var item = await _db.Enrolments.Include(e => e.Event).SingleOrDefaultAsync(e => e.EnrolmentId == id && e.Event.OrganiserId == UserId);
        if (item == null) return NotFound(); item.Status = request.Status; await _db.SaveChangesAsync(); return NoContent();
    }
}