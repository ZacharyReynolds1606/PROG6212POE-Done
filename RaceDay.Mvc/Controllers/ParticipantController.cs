using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Events;
using RaceDay.Contracts.Enrolments;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers;

public class ParticipantController : Controller
{
    private readonly RaceDayApiClient _api;
    public ParticipantController(RaceDayApiClient api) => _api = api;
    private bool IsParticipant => HttpContext.Session.GetString("Role") == "Participant";
    private IActionResult Login() => RedirectToAction("Login", "Auth");

    public async Task<IActionResult> Index(string? type, string? search)
    {
        if (!IsParticipant) return Login();
        var response = await _api.SendAsync(HttpMethod.Get, "api/events");
        var events = await RaceDayApiClient.ReadAsync<List<EventDto>>(response) ?? new();
        events = events.Where(e => e.EventDate >= DateTime.Today).Where(e => string.IsNullOrWhiteSpace(type) || e.EventType == type).Where(e => string.IsNullOrWhiteSpace(search) || e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || e.Location.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        return View(events);
    }
    public async Task<IActionResult> MyEvents()
    {
        if (!IsParticipant) return Login();
        var response = await _api.SendAsync(HttpMethod.Get, "api/enrolments/mine");
        return View(await RaceDayApiClient.ReadAsync<List<EnrolmentDto>>(response) ?? new());
    }

    public async Task<IActionResult> MyResults()
    {
        if (!IsParticipant) return Login();
        var response = await _api.SendAsync(HttpMethod.Get, "api/results/mine");
        return View(await RaceDayApiClient.ReadAsync<List<RaceDay.Contracts.Results.ResultDto>>(response) ?? new());
    }

    public async Task<IActionResult> Profile()
    {
        if (!IsParticipant) return Login();
        var response = await _api.SendAsync(HttpMethod.Get, "api/profile/me");
        return View(await RaceDayApiClient.ReadAsync<RaceDay.Contracts.Profiles.ProfileDto>(response));
    }

    [HttpPost]
    public async Task<IActionResult> EnterEvent(int eventId, int categoryId)
    {
        if (!IsParticipant) return Login();
        await _api.SendAsync(HttpMethod.Post, "api/enrolments", new CreateEnrolmentRequest { EventId = eventId, CategoryId = categoryId });
        return RedirectToAction(nameof(MyEvents));
    }
}