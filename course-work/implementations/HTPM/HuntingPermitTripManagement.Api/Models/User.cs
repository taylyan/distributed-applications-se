using System.ComponentModel.DataAnnotations;

namespace HuntingPermitTripManagement.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "Hunter";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
    public ICollection<HuntingTrip> HuntingTrips { get; set; } = new List<HuntingTrip>();
}