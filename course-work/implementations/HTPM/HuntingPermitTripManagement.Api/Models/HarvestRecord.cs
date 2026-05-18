using System.ComponentModel.DataAnnotations;

namespace HuntingPermitTripManagement.Api.Models;

public class HarvestRecord
{
    public int Id { get; set; }

    [Required]
    public int TripId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AnimalType { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public double Weight { get; set; }

    public bool IsLegal { get; set; } = true;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public HuntingTrip? Trip { get; set; }
}