using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HarvestRecordsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HarvestRecordsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HarvestRecord>>> GetHarvestRecords()
    {
        var records = await _context.HarvestRecords
            .Include(r => r.Trip)
            .ToListAsync();

        return Ok(records);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HarvestRecord>> GetHarvestRecord(int id)
    {
        var record = await _context.HarvestRecords
            .Include(r => r.Trip)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound();
        }

        return Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<HarvestRecord>> CreateHarvestRecord(HarvestRecord record)
    {
        var tripExists = await _context.HuntingTrips
            .AnyAsync(t => t.Id == record.TripId);

        if (!tripExists)
        {
            return BadRequest("Trip does not exist.");
        }

        if (record.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than 0.");
        }

        if (record.Weight <= 0)
        {
            return BadRequest("Weight must be greater than 0.");
        }

        _context.HarvestRecords.Add(record);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetHarvestRecord),
            new { id = record.Id },
            record);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHarvestRecord(int id, HarvestRecord record)
    {
        if (id != record.Id)
        {
            return BadRequest();
        }

        _context.Entry(record).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHarvestRecord(int id)
    {
        var record = await _context.HarvestRecords.FindAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        _context.HarvestRecords.Remove(record);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}