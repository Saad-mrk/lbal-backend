using Domain.Entities;
using LBAL.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Categorie> CATEGORIE { get; set; }
    public DbSet<SousCategorie> SOUS_CATEGORIE { get; set; }
    public DbSet<Annonce> Annonces { get; set; }
    public DbSet<PhotoAnnonce> PhotoAnnonces { get; set; }
    public DbSet<AnnonceAttribut> AnnonceAttributs { get; set; }
    public DbSet<Attribut> Attributs { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de l'entité User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Prenom).IsRequired().HasMaxLength(50);
        });
        // Configuration des Enums
        modelBuilder.Entity<Annonce>()
            .Property(e => e.EtatId)
            .HasConversion<int>(); // Ou supprimez cette ligne si c'est déjà un int
        modelBuilder.Entity<Annonce>()
            .Property(e => e.id_utilisateur)
            .HasColumnName("id_utilisateur")  // ← Nom exact
            .IsRequired();

        modelBuilder.Entity<Annonce>()
            .Property(e => e.StatutId)
            .HasConversion<int>(); // Ou supprimez cette ligne si c'est déjà un int

        // Configuration des relations (Cascade Delete)
        modelBuilder.Entity<Annonce>()
            .HasMany(a => a.Photos)
            .WithOne(p => p.Annonce)
            .HasForeignKey(p => p.IdAnnonce)
            .OnDelete(DeleteBehavior.Cascade);
    }
}