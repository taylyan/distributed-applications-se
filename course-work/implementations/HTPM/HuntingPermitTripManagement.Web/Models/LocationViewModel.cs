namespace HuntingPermitTripManagement.Web.Models;

public class LocationViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double AreaSize { get; set; }

    public bool IsProtected { get; set; }
}