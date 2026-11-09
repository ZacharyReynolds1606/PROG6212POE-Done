using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Results;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/results")]
public class ResultsController :ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;
    private int? UserId => HttpContext.Session.GetInt32("UserId");
    [HttpPost]
    public async Task<IActionResult> CreateResult([FromBody] CreateResultRequest request)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser" || UserId == null) return Unauthorized();
        var enrolment = await _db.Enrolments.Include(e => e.Event).SingleOrDefaultAsync(e => e.EnrolmentId == request.EnrolmentId && e.Event.OrganiserId == UserId);
        if (enrolment == null) return NotFound();
        _db.Results.Add(new Result { EnrolmentId = request.EnrolmentId, FinishTime = request.FinishTime, Position = request.Position }); await _db.SaveChangesAsync(); return Created("api/results", null);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> MyResults()
    {
        if (UserId == null) return Unauthorized();
        return Ok(await _db.Results.AsNoTracking().Where(r => r.Enrolment.ParticipantId == UserId).Include(r => r.Enrolment).ThenInclude(e => e.Event).Include(r => r.Enrolment).ThenInclude(e => e.Category).Select(r => new ResultDto { EventName = r.Enrolment.Event.Name, CategoryName = r.Enrolment.Category.Name, FinishTime = r.FinishTime, Position = r.Position }).ToListAsync());
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> EventResults(int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser") return Unauthorized();
        return Ok(await _db.Results.AsNoTracking().Where(r => r.Enrolment.EventId == eventId).Include(r => r.Enrolment).ThenInclude(e => e.Event).Include(r => r.Enrolment).ThenInclude(e => e.Category).Select(r => new ResultDto { EventName = r.Enrolment.Event.Name, CategoryName = r.Enrolment.Category.Name, FinishTime = r.FinishTime, Position = r.Position }).ToListAsync());
    }
}
