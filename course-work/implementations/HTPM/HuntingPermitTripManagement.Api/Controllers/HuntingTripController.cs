using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HuntingTripsController : BaseApiController
{
    private readonly ApplicationDbContext _context;

    public HuntingTripsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HuntingTrip>>> GetTrips()
    {
        var query = _context.HuntingTrips
        .Include(t => t.User)
        .Include(t => t.Location)
        .Include(t => t.Permit)
        .AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(t => t.UserId == CurrentUserId);
        }

        var trips = await query.ToListAsync();

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
        if (!IsAdmin && trip.UserId != CurrentUserId)
        {
            return Forbid();
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

        if (!IsAdmin && trip.UserId != CurrentUserId)
        {
            return Forbid();
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

        var existingTrip = await _context.HuntingTrips
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTrip == null)
        {
            return NotFound();
        }

        if (!IsAdmin && existingTrip.UserId != CurrentUserId)
        {
            return Forbid();
        }

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

        if (!IsAdmin && trip.UserId != CurrentUserId)
        {
            return Forbid();
        }

        existingTrip.UserId = trip.UserId;
        existingTrip.LocationId = trip.LocationId;
        existingTrip.PermitId = trip.PermitId;
        existingTrip.TripDate = trip.TripDate;
        existingTrip.DurationHours = trip.DurationHours;
        existingTrip.Notes = trip.Notes;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var trip = await _context.HuntingTrips
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trip == null)
        {
            return NotFound();
        }

        if (!IsAdmin && trip.UserId != CurrentUserId)
        {
            return Forbid();
        }

        _context.HuntingTrips.Remove(trip);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}