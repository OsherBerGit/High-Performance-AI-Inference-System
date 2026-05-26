using Microsoft.EntityFrameworkCore;
using Gateway.Models;

namespace Gateway.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<UserBaseline> UserBaselines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.AuditLogs)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Baseline)
            .WithOne(b => b.User)
            .HasForeignKey<UserBaseline>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}