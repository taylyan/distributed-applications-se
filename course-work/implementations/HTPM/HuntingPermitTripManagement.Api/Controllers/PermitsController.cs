using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermitsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PermitsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Permit>>> GetPermits()
    {
        var permits = await _context.Permits
            .Include(p => p.User)
            .ToListAsync();

        return Ok(permits);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Permit>> GetPermit(int id)
    {
        var permit = await _context.Permits
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (permit == null)
        {
            return NotFound();
        }

        return Ok(permit);
    }

    [HttpPost]
    public async Task<ActionResult<Permit>> CreatePermit(Permit permit)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == permit.UserId);

        if (!userExists)
        {
            return BadRequest("User does not exist.");
        }

        if (permit.ExpirationDate <= permit.IssueDate)
        {
            return BadRequest("Expiration date must be after issue date.");
        }

        _context.Permits.Add(permit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPermit),
            new { id = permit.Id },
            permit);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePermit(int id, Permit permit)
    {
        if (id != permit.Id)
        {
            return BadRequest();
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == permit.UserId);

        if (!userExists)
        {
            return BadRequest("User does not exist.");
        }

        if (permit.ExpirationDate <= permit.IssueDate)
        {
            return BadRequest("Expiration date must be after issue date.");
        }

        _context.Entry(permit).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermit(int id)
    {
        var permit = await _context.Permits.FindAsync(id);

        if (permit == null)
        {
            return NotFound();
        }

        _context.Permits.Remove(permit);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}