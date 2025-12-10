using Microsoft.EntityFrameworkCore;
using OKServer.Models;

namespace OKServer.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<Heartbeat> Heartbeats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Device configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.OwnerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasIndex(e => e.OwnerEmail);
        });

        // Heartbeat configuration
        modelBuilder.Entity<Heartbeat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeviceId).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.Timestamp).IsRequired();
            
            entity.HasOne(e => e.Device)
                .WithMany(d => d.Heartbeats)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}

