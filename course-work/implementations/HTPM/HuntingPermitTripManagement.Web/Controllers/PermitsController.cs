using HuntingPermitTripManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HuntingPermitTripManagement.Web.Controllers;

public class PermitsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PermitsController(IHttpClientFactory httpClientFactory)
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

        var response = await client.GetAsync("Permits");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not load permits.";
            return View(new List<PermitViewModel>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var permits = JsonSerializer.Deserialize<List<PermitViewModel>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(permits ?? new List<PermitViewModel>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PermitViewModel
        {
            IssueDate = DateTime.Today,
            ExpirationDate = DateTime.Today.AddYears(1),
            IsActive = true
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(PermitViewModel model)
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

        var response = await client.PostAsync("Permits", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not create permit. Make sure User Id exists and dates are valid.";
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

        var response = await client.GetAsync($"Permits/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var permit = JsonSerializer.Deserialize<PermitViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(permit);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PermitViewModel model)
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

        var response = await client.PutAsync($"Permits/{model.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Could not update permit. Make sure User Id exists and dates are valid.";
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

        var response = await client.GetAsync($"Permits/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var json = await response.Content.ReadAsStringAsync();

        var permit = JsonSerializer.Deserialize<PermitViewModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return View(permit);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = CreateAuthorizedClient();

        if (client == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await client.DeleteAsync($"Permits/{id}");

        return RedirectToAction("Index");
    }
}