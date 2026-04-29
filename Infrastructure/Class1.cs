using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LBAL.Domain.Entities;
public class Utilisateur
{
    [Key]
    public int IdUtilisateur { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string MotDePasseHash { get; set; }

    public string Nom { get; set; }

    public string Prenom { get; set; }

    public string Telephone { get; set; }

    // Clé étrangère vers une table Role
    public int RoleId { get; set; }

    public bool EstActif { get; set; }

    public DateTime DateInscription { get; set; }

    public DateTime? DerniereConnexion { get; set; } // Nullable car peut être vide au début

    public DateTime? DeletedAt { get; set; } // Nullable pour le "Soft Delete"
    public bool IsVerified { get; set; } = false;
    public string? VerificationCode { get; set; }
    public DateTime? CodeExpiresAt { get; set; }

    // Utile pour la sécurité du rafraîchissement de jeton plus tard
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}