namespace HuntingPermitTripManagement.Web.Models;

public class HarvestRecordViewModel
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string AnimalType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public double Weight { get; set; }

    public bool IsLegal { get; set; }

    public DateTime RecordedAt { get; set; }
}