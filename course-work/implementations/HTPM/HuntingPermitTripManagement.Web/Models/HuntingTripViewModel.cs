namespace HuntingPermitTripManagement.Web.Models;

public class HuntingTripViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LocationId { get; set; }

    public int PermitId { get; set; }

    public DateTime TripDate { get; set; }

    public int DurationHours { get; set; }

    public string? Notes { get; set; }
}