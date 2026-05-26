using HuntingPermitTripManagement.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : BaseApiController
{
    private readonly ApplicationDbContext _context;

    public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("harvest")]
    public async Task<IActionResult> GetHarvestStatistics( DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.HarvestRecords
        .Include(h => h.Trip)
        .AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(h => h.Trip!.UserId == CurrentUserId);
        }

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
        var query = _context.HuntingTrips
        .Include(t => t.Location)
        .AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(t => t.UserId == CurrentUserId);
        }

        var totalTrips = await query.CountAsync();


        var mostVisitedLocation = await query
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
        var query = _context.Permits.AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(p => p.UserId == CurrentUserId);
        }

        var totalPermits = await query.CountAsync();
        var activePermits = await query.CountAsync(p => p.IsActive);
        var expiredPermits = await query.CountAsync(p => p.ExpirationDate < DateTime.UtcNow);

        return Ok(new
        {
            totalPermits,
            activePermits,
            expiredPermits
        });
    }
}