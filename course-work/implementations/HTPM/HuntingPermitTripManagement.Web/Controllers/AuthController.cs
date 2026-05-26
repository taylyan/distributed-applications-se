using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HuntingPermitTripManagement.Web.Controllers;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("Auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        var responseJson = await response.Content.ReadAsStringAsync();

        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
            responseJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        HttpContext.Session.SetString("JwtToken", loginResponse!.Token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(loginResponse.Token);

        var role = jwtToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")
            ?.Value;

        var userId = jwtToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid")
            ?.Value;

        if (!string.IsNullOrEmpty(role))
        {
            HttpContext.Session.SetString("UserRole", role);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            HttpContext.Session.SetString("UserId", userId);
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JwtToken");
        HttpContext.Session.Remove("UserRole");
        HttpContext.Session.Remove("UserId");

        return RedirectToAction("Login");
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}