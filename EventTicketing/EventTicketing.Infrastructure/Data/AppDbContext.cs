using EventTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Event> Events { get; set; }
  public DbSet<Ticket> Tickets { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Event>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Name)
              .IsRequired()
              .HasMaxLength(200);

      entity.Property(e => e.Description)
              .HasMaxLength(1000);

      entity.Property(e => e.Location)
              .IsRequired()
              .HasMaxLength(300);

      entity.Property(e => e.BasePrice)
              .HasPrecision(18, 2); // Decimal with 2 decimal places

      entity.HasMany(e => e.Tickets)
              .WithOne(t => t.Event)
              .HasForeignKey(t => t.EventId)
              .OnDelete(DeleteBehavior.Cascade); // Delete tickets when event is deleted
    });

    // Configure Ticket entity
    modelBuilder.Entity<Ticket>(entity =>
    {
      entity.HasKey(t => t.Id);

      entity.Property(t => t.AttendeeEmail)
              .IsRequired()
              .HasMaxLength(100);

      entity.Property(t => t.AttendeeFullName)
              .IsRequired()
              .HasMaxLength(200);

      entity.Property(t => t.SeatNumber)
              .IsRequired()
              .HasMaxLength(20);

      entity.Property(t => t.PricePaid)
              .HasPrecision(18, 2);

      entity.Property(t => t.Status)
              .HasConversion<int>(); // Store enum as integer
    });
  }
}