using HuntingPermitTripManagement.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("harvest")]
    public async Task<IActionResult> GetHarvestStatistics(
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.HarvestRecords.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(h => h.RecordedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(h => h.RecordedAt <= toDate.Value);
        }

        var totalAnimals = await query.SumAsync(h => h.Quantity);
        var totalWeight = await query.SumAsync(h => h.Weight);

        var mostHuntedAnimal = await query
            .GroupBy(h => h.AnimalType)
            .Select(g => new
            {
                AnimalType = g.Key,
                TotalQuantity = g.Sum(h => h.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .FirstOrDefaultAsync();

        return Ok(new
        {
            totalAnimals,
            totalWeight,
            mostHuntedAnimal = mostHuntedAnimal?.AnimalType
        });
    }

    [HttpGet("trips")]
    public async Task<IActionResult> GetTripStatistics()
    {
        var totalTrips = await _context.HuntingTrips.CountAsync();

        var mostVisitedLocation = await _context.HuntingTrips
            .Include(t => t.Location)
            .GroupBy(t => t.Location!.Name)
            .Select(g => new
            {
                Location = g.Key,
                TotalTrips = g.Count()
            })
            .OrderByDescending(x => x.TotalTrips)
            .FirstOrDefaultAsync();

        return Ok(new
        {
            totalTrips,
            mostVisitedLocation = mostVisitedLocation?.Location
        });
    }

    [HttpGet("permits")]
    public async Task<IActionResult> GetPermitStatistics()
    {
        var totalPermits = await _context.Permits.CountAsync();
        var activePermits = await _context.Permits.CountAsync(p => p.IsActive);
        var expiredPermits = await _context.Permits
            .CountAsync(p => p.ExpirationDate < DateTime.UtcNow);

        return Ok(new
        {
            totalPermits,
            activePermits,
            expiredPermits
        });
    }
}