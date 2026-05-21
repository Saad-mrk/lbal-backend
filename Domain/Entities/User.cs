using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Domain.Entities;

[Table("Users", Schema = "dbo")] // On cible ta table exacte
public class User
{
    [Key]
    [Column("id_utilisateur")]
    public int Id { get; set; }

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("mot_de_passe_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("nom")]
    public string? Nom { get; set; }

    [Column("prenom")]
    public string? Prenom { get; set; }

    [Column("telephone")]
    public string? Telephone { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; } = 1; // 1 correspond à 'user' dans ta table des rôles

    [Column("est_actif")]
    public bool? EstActif { get; set; } = true;

    [Column("date_inscription")]
    public DateTime? DateInscription { get; set; } = DateTime.UtcNow;

    [Column("derniere_connexion")]
    public DateTime? DerniereConnexion { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    // --- LES CHAMPS POUR L'EMAIL ---
    // [NotMapped] dit à Entity Framework : "N'essaie pas d'enregistrer ça dans SQL Server"

    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;

    [Column("verification_code")]
    public string? VerificationCode { get; set; }

    [Column("code_expires_at")]
    public DateTime? CodeExpiresAt { get; set; }
  
    // refresh token

    [Column("refreshtokenhash")]
    public string? RefreshTokenHash { get; set; }

    [Column("refreshtokenexpiresAt")]
    public DateTime? RefreshTokenExpiresAt { get; set; }

    [Column("refreshtokenrevokedat")]
    public DateTime? RefreshTokenRevokedAt { get; set; }

    // Relations (Propriétés de navigation)
    public List<Annonce> Annonces { get; set; } = new();
   // public List<Adresse> Adresses { get; set; } = new();
   // public List<Favori> Favoris { get; set; } = new();

}