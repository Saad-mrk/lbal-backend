using Microsoft.EntityFrameworkCore;
using LBAL.Domain.Entities;

namespace LBAL.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de l'entité User
        modelBuilder.Entity<User>(entity => {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Prenom).IsRequired().HasMaxLength(50);
        });
    }
}