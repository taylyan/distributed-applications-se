using System.ComponentModel.DataAnnotations;

namespace HuntingPermitTripManagement.Api.Models;

public class HuntingTrip
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public int PermitId { get; set; }

    [Required]
    public DateTime TripDate { get; set; }

    [Required]
    public int DurationHours { get; set; }

    [MaxLength(255)]
    public string? Notes { get; set; }

    public User? User { get; set; }
    public Location? Location { get; set; }
    public Permit? Permit { get; set; }

    public ICollection<HarvestRecord> HarvestRecords { get; set; } = new List<HarvestRecord>();
}