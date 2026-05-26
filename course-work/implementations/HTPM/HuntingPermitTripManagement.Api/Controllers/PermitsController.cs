using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermitsController : BaseApiController 
{
    private readonly ApplicationDbContext _context;

    public PermitsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Permit>>> GetPermits()
    {
        var query = _context.Permits
        .Include(p => p.User)
        .AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(p => p.UserId == CurrentUserId);
        }

        var permits = await query.ToListAsync();
      

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

        if (!IsAdmin && permit.UserId != CurrentUserId)
        {
            return Forbid();
        }
        return Ok(permit);
    }

    [HttpPost]
    public async Task<ActionResult<Permit>> CreatePermit(Permit permit)
    {
        if (!IsAdmin)
        {
            return Forbid();
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
        if (!IsAdmin)
        {
            return Forbid();
        }

        if (id != permit.Id)
        {
            return BadRequest();
        }

        var existingPermit = await _context.Permits
       .FirstOrDefaultAsync(p => p.Id == id);

        if (existingPermit == null)
        {
            return NotFound();
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

        existingPermit.UserId = permit.UserId;
        existingPermit.PermitNumber = permit.PermitNumber;
        existingPermit.IssueDate = permit.IssueDate;
        existingPermit.ExpirationDate = permit.ExpirationDate;
        existingPermit.IsActive = permit.IsActive;
        existingPermit.PermitType = permit.PermitType;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermit(int id)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

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