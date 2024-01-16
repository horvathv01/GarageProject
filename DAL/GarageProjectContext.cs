using Microsoft.EntityFrameworkCore;
using GarageProject.Models;
using PsychAppointments_API.Models;

namespace GarageProject.DAL;

public class GarageProjectContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Manager> Managers { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<ParkingSpace> ParkingSpaces { get; set; }



    public GarageProjectContext(DbContextOptions<GarageProjectContext> contextOptions) : base(contextOptions)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasOne( b => b.User )
            .WithMany( u => u.Bookings )
            .HasForeignKey( b => b.UserId );

        modelBuilder.Entity<Booking>()
            .HasOne( b => b.ParkingSpace )
            .WithMany()
            .HasForeignKey( b => b.ParkingSpaceId );

        modelBuilder.Entity<User>()
            .HasMany( u => u.Bookings )
            .WithOne( b => b.User );
    }
}