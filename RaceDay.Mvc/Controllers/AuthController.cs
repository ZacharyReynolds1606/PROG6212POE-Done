using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using RaceDay.Contracts.Auth;

namespace RaceDay.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly RaceDayApiClient _api;

    public AuthController(RaceDayApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var response = await _api.SendAsync(HttpMethod.Post, "api/auth/login", new LoginRequest { Email = vm.Email, Password = vm.Password });
        if (!response.IsSuccessStatusCode) { ModelState.AddModelError("", "Invalid email or password."); return View(vm); }
        var auth = await RaceDayApiClient.ReadAsync<AuthResponse>(response);
        if (auth == null) { ModelState.AddModelError("", "The login response was invalid."); return View(vm); }
        var cookie = RaceDayApiClient.ExtractSessionCookie(response);
        if (cookie != null) HttpContext.Session.SetString("ApiSessionCookie", cookie);
        HttpContext.Session.SetInt32("UserId", auth.UserId);
        HttpContext.Session.SetString("Role", auth.Role);
        HttpContext.Session.SetString("DisplayName", auth.FullName);
        return RedirectToAction("Index", auth.Role == "Organiser" ? "Organiser" : "Participant");
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var request = new RegisterRequest
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Password = vm.Password,
            Role = vm.Role
        };

        var response =
            await _api.SendAsync(
                HttpMethod.Post,
                "api/auth/register",
                request);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                "",
                "Registration failed.");

            return View(vm);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _api.SendAsync(HttpMethod.Post, "api/auth/logout");
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}