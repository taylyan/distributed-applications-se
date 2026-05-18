using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HuntingPermitTripManagement.Web.Controllers;

public class UsersController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UsersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var token = HttpContext.Session.GetString("JwtToken");

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                token);

        var response = await client.GetAsync("Users");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load users.";
            return View(new List<UserViewModel>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var users = JsonSerializer.Deserialize<List<UserViewModel>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(users ?? new List<UserViewModel>());
    }
}