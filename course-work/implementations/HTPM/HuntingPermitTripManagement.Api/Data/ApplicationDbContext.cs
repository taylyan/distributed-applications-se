using HuntingPermitTripManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HuntingPermitTripManagement.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Permit> Permits => Set<Permit>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<HuntingTrip> HuntingTrips => Set<HuntingTrip>();
    public DbSet<HarvestRecord> HarvestRecords => Set<HarvestRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Permit>()
            .HasOne(p => p.User)
            .WithMany(u => u.Permits)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HuntingTrip>()
            .HasOne(t => t.User)
            .WithMany(u => u.HuntingTrips)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HuntingTrip>()
            .HasOne(t => t.Location)
            .WithMany(l => l.HuntingTrips)
            .HasForeignKey(t => t.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HuntingTrip>()
            .HasOne(t => t.Permit)
            .WithMany()
            .HasForeignKey(t => t.PermitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HarvestRecord>()
            .HasOne(h => h.Trip)
            .WithMany(t => t.HarvestRecords)
            .HasForeignKey(h => h.TripId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}