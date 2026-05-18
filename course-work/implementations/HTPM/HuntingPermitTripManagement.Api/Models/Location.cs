using System.ComponentModel.DataAnnotations;

namespace HuntingPermitTripManagement.Api.Models;

public class Location
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public double AreaSize { get; set; }

    public bool IsProtected { get; set; }

    public ICollection<HuntingTrip> HuntingTrips { get; set; } = new List<HuntingTrip>();
}