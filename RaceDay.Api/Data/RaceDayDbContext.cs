using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(
        DbContextOptions<RaceDayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }

    public DbSet<AppUser> Users { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Enrolment> Enrolments { get; set; }

    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().Property(e => e.DistanceKm).HasPrecision(10, 2);
        base.OnModelCreating(modelBuilder);
    }
}