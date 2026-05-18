using HuntingPermitTripManagement.Api.Data;
using HuntingPermitTripManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HuntingPermitTripManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers(
    string? firstName,
    string? lastName,
    int pageNumber = 1,
    int pageSize = 10,
    string? sortBy = "id",
    string? sortDirection = "asc")
    {
        var query = _context.Users.AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(firstName))
        {
            query = query.Where(u =>
                u.FirstName.Contains(firstName));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(u =>
                u.LastName.Contains(lastName));
        }

        // Sorting
        query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
        {
            ("firstname", "desc") => query.OrderByDescending(u => u.FirstName),
            ("firstname", _) => query.OrderBy(u => u.FirstName),

            ("lastname", "desc") => query.OrderByDescending(u => u.LastName),
            ("lastname", _) => query.OrderBy(u => u.LastName),

            ("createdat", "desc") => query.OrderByDescending(u => u.CreatedAt),
            ("createdat", _) => query.OrderBy(u => u.CreatedAt),

            _ => query.OrderBy(u => u.Id)
        };

        // Pagination
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/users/1
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            user);
    }

    // PUT: api/users/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/users/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}