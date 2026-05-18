namespace HuntingPermitTripManagement.Web.Models;

public class StatisticsViewModel
{
    public HarvestStatisticsViewModel Harvest { get; set; } = new();

    public TripStatisticsViewModel Trips { get; set; } = new();

    public PermitStatisticsViewModel Permits { get; set; } = new();
}

public class HarvestStatisticsViewModel
{
    public int TotalAnimals { get; set; }

    public double TotalWeight { get; set; }

    public string? MostHuntedAnimal { get; set; }
}

public class TripStatisticsViewModel
{
    public int TotalTrips { get; set; }

    public string? MostVisitedLocation { get; set; }
}

public class PermitStatisticsViewModel
{
    public int TotalPermits { get; set; }

    public int ActivePermits { get; set; }

    public int ExpiredPermits { get; set; }
}