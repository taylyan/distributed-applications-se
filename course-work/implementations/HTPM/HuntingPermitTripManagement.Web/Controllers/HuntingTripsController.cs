using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HuntingPermitTripManagement.Web.Controllers;

public class HuntingTripsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HuntingTripsController(IHttpClientFactory httpClientFactory)
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

        var response = await client.GetAsync("HuntingTrips");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load hunting trips.";
            return View(new List<HuntingTripViewModel>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var trips = JsonSerializer.Deserialize<List<HuntingTripViewModel>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(trips ?? new List<HuntingTripViewModel>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new HuntingTripViewModel
        {
            TripDate = DateTime.Today,
            DurationHours = 1
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(HuntingTripViewModel model)
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

        var response = await client.PostAsync("HuntingTrips", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not create hunting trip. Make sure User Id, Location Id and Permit Id exist.";
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

        var response = await client.GetAsync($"HuntingTrips/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var trip = JsonSerializer.Deserialize<HuntingTripViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(trip);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HuntingTripViewModel model)
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

        var response = await client.PutAsync($"HuntingTrips/{model.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not update hunting trip. Make sure User Id, Location Id and Permit Id exist.";
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

        var response = await client.GetAsync($"HuntingTrips/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var trip = JsonSerializer.Deserialize<HuntingTripViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(trip);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await client.DeleteAsync($"HuntingTrips/{id}");

        return RedirectToAction("Index");
    }
}