using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationsController : BaseApiController
{
    private readonly ApplicationDbContext _context;

    public LocationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
    {
        var locations = await _context.Locations.ToListAsync();

        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Location>> GetLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);

        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPost]
    public async Task<ActionResult<Location>> CreateLocation(Location location)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

        _context.Locations.Add(location);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetLocation),
            new { id = location.Id },
            location);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, Location location)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

        if (id != location.Id)
        {
            return BadRequest();
        }

        _context.Entry(location).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

        var location = await _context.Locations.FindAsync(id);

        if (location == null)
        {
            return NotFound();
        }

        _context.Locations.Remove(location);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}