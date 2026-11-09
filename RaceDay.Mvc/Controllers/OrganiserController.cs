using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Events;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class OrganiserController : Controller
{
    private readonly RaceDayApiClient _api;
    public OrganiserController(RaceDayApiClient api) => _api = api;
    private bool IsOrganiser => HttpContext.Session.GetString("Role") == "Organiser";
    private IActionResult Login() => RedirectToAction("Login", "Auth");

    public async Task<IActionResult> Index()
    {
        if (!IsOrganiser) return Login();
        var response = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
        return View(await RaceDayApiClient.ReadAsync<List<EventDto>>(response) ?? new());
    }
    public IActionResult CreateEvent()
    {
        return IsOrganiser ? View(new CreateEventRequest { EventDate = DateTime.Today }) : Login();
    }
    public async Task<IActionResult> Events()
    {
        if (!IsOrganiser) return Login();
        var response = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
        return View(await RaceDayApiClient.ReadAsync<List<EventDto>>(response) ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent(CreateEventRequest model)
    {
        if (!IsOrganiser) return Login();
        if (!ModelState.IsValid) return View(model);
        var response = await _api.SendAsync(HttpMethod.Post, "api/events", model);
        if (!response.IsSuccessStatusCode) { ModelState.AddModelError("", "The event could not be created."); return View(model); }
        return RedirectToAction(nameof(Events));
    }

    public IActionResult Enrolments()
    {
        return IsOrganiser ? View() : Login();
    }

    public IActionResult Results()
    {
        return IsOrganiser ? View() : Login();
    }
}