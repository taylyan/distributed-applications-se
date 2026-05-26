using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HuntingPermitTripManagement.Web.Controllers;

public class LocationsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LocationsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient? CreateAuthorizedClient()
    {
        var token = HttpContext.Session.GetString("JwtToken");

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    public async Task<IActionResult> Index()
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await client.GetAsync("Locations");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load locations.";
            return View(new List<LocationViewModel>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var locations = JsonSerializer.Deserialize<List<LocationViewModel>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(locations ?? new List<LocationViewModel>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new LocationViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(LocationViewModel model)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var json = JsonSerializer.Serialize(model);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("Locations", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not create location.";
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await client.GetAsync($"Locations/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var location = JsonSerializer.Deserialize<LocationViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(location);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LocationViewModel model)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var json = JsonSerializer.Serialize(model);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync($"Locations/{model.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not update location.";
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await client.GetAsync($"Locations/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var location = JsonSerializer.Deserialize<LocationViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(location);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await client.DeleteAsync($"Locations/{id}");

        return RedirectToAction("Index");
    }
}