using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HuntingPermitTripManagement.Web.Controllers;

public class StatisticsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StatisticsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JwtToken");

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var model = new StatisticsViewModel();

        var harvestResponse = await client.GetAsync("Statistics/harvest");
        if (harvestResponse.IsSuccessStatusCode)
        {
            var json = await harvestResponse.Content.ReadAsStringAsync();
            model.Harvest = JsonSerializer.Deserialize<HarvestStatisticsViewModel>(json, options) ?? new();
        }

        var tripsResponse = await client.GetAsync("Statistics/trips");
        if (tripsResponse.IsSuccessStatusCode)
        {
            var json = await tripsResponse.Content.ReadAsStringAsync();
            model.Trips = JsonSerializer.Deserialize<TripStatisticsViewModel>(json, options) ?? new();
        }

        var permitsResponse = await client.GetAsync("Statistics/permits");
        if (permitsResponse.IsSuccessStatusCode)
        {
            var json = await permitsResponse.Content.ReadAsStringAsync();
            model.Permits = JsonSerializer.Deserialize<PermitStatisticsViewModel>(json, options) ?? new();
        }

        return View(model);
    }
}