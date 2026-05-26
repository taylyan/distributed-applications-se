using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HuntingPermitTripManagement.Web.Controllers;

public class HarvestRecordsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HarvestRecordsController(IHttpClientFactory httpClientFactory)
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

        var response = await client.GetAsync("HarvestRecords");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load harvest records.";
            return View(new List<HarvestRecordViewModel>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var records = JsonSerializer.Deserialize<List<HarvestRecordViewModel>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(records ?? new List<HarvestRecordViewModel>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new HarvestRecordViewModel
        {
            Quantity = 1,
            Weight = 1,
            IsLegal = true,
            RecordedAt = DateTime.Today
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(HarvestRecordViewModel model)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var requestBody = new
        {
            tripId = model.TripId,
            animalType = model.AnimalType,
            quantity = model.Quantity,
            weight = model.Weight,
            isLegal = model.IsLegal,
            recordedAt = model.RecordedAt
        };

        var json = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("HarvestRecords", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not create harvest record. Make sure Trip Id exists, Quantity > 0 and Weight > 0.";
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

        var response = await client.GetAsync($"HarvestRecords/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var record = JsonSerializer.Deserialize<HarvestRecordViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(record);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HarvestRecordViewModel model)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var requestBody = new
        {
            id = model.Id,
            tripId = model.TripId,
            animalType = model.AnimalType,
            quantity = model.Quantity,
            weight = model.Weight,
            isLegal = model.IsLegal,
            recordedAt = model.RecordedAt
        };

        var json = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync($"HarvestRecords/{model.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not update harvest record. Make sure Trip Id exists, Quantity > 0 and Weight > 0.";
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

        var response = await client.GetAsync($"HarvestRecords/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var record = JsonSerializer.Deserialize<HarvestRecordViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(record);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await client.DeleteAsync($"HarvestRecords/{id}");

        return RedirectToAction("Index");
    }
}