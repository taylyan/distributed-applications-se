using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HarvestRecordsController : BaseApiController
{
    private readonly ApplicationDbContext _context;

    public HarvestRecordsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HarvestRecord>>> GetHarvestRecords()
    {
        var query = _context.HarvestRecords
        .Include(r => r.Trip)
        .AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(r => r.Trip!.UserId == CurrentUserId);
        }

        var records = await query.ToListAsync();

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

        if (!IsAdmin && record.Trip!.UserId != CurrentUserId)
        {
            return Forbid();
        }

        return Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<HarvestRecord>> CreateHarvestRecord(HarvestRecord record)
    {
        var trip = await _context.HuntingTrips
        .FirstOrDefaultAsync(t => t.Id == record.TripId);

        if (trip == null)
        {
            return BadRequest("Trip does not exist.");
        }

        if (!IsAdmin && trip.UserId != CurrentUserId)
        {
            return Forbid();
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

        var existingRecord = await _context.HarvestRecords
            .Include(r => r.Trip)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRecord == null)
        {
            return NotFound();
        }

        var trip = await _context.HuntingTrips
            .FirstOrDefaultAsync(t => t.Id == record.TripId);

        if (trip == null)
        {
            return BadRequest("Trip does not exist.");
        }

        if (!IsAdmin && trip.UserId != CurrentUserId)
        {
            return Forbid();
        }

        if (record.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than 0.");
        }

        if (record.Weight <= 0)
        {
            return BadRequest("Weight must be greater than 0.");
        }

        existingRecord.TripId = record.TripId;
        existingRecord.AnimalType = record.AnimalType;
        existingRecord.Quantity = record.Quantity;
        existingRecord.Weight = record.Weight;
        existingRecord.IsLegal = record.IsLegal;
        existingRecord.RecordedAt = record.RecordedAt;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHarvestRecord(int id)
    {
        var record = await _context.HarvestRecords
            .Include(r => r.Trip)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound();
        }

        if (!IsAdmin && record.Trip!.UserId != CurrentUserId)
        {
            return Forbid();
        }

        _context.HarvestRecords.Remove(record);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}