using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HuntingTripsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HuntingTripsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HuntingTrip>>> GetTrips()
    {
        var trips = await _context.HuntingTrips
            .Include(t => t.User)
            .Include(t => t.Location)
            .Include(t => t.Permit)
            .ToListAsync();

        return Ok(trips);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HuntingTrip>> GetTrip(int id)
    {
        var trip = await _context.HuntingTrips
            .Include(t => t.User)
            .Include(t => t.Location)
            .Include(t => t.Permit)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip == null)
        {
            return NotFound();
        }

        return Ok(trip);
    }

    [HttpPost]
    public async Task<ActionResult<HuntingTrip>> CreateTrip(HuntingTrip trip)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == trip.UserId);

        var permitExists = await _context.Permits.AnyAsync(p => p.Id == trip.PermitId);

        var locationExists = await _context.Locations.AnyAsync(l => l.Id == trip.LocationId);

        if (!userExists)
        {
            return BadRequest("User does not exist.");
        }

        if (!permitExists)
        {
            return BadRequest("Permit does not exist.");
        }

        if (!locationExists)
        {
            return BadRequest("Location does not exist.");
        }

        _context.HuntingTrips.Add(trip);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTrip),
            new { id = trip.Id },
            trip);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTrip(int id, HuntingTrip trip)
    {
        if (id != trip.Id)
        {
            return BadRequest();
        }

        _context.Entry(trip).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var trip = await _context.HuntingTrips.FindAsync(id);

        if (trip == null)
        {
            return NotFound();
        }

        _context.HuntingTrips.Remove(trip);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}