using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;


namespace Domain.Entities // Utilise le namespace standard de ton projet
{
        [Table("ANNONCE_ATTRIBUT", Schema = "dbo")] // On cible ta table exacte
    public class AnnonceAttribut
    {
        [Key]
        [Column("id ")]
        public int Id { get; set; }
        [Required]
        [Column("id_annonce ")]
        public int IdAnnonce { get; set; }
        [Required]
        [Column("id_attribut ")]
        public int IdAttribut { get; set; }

        [Required]
        [StringLength(255)]
        [Column("valeur ")]

        public string Valeur { get; set; } = string.Empty;

        // Propriétés de navigation (EF Core)a
        // Le "null!" indique au compilateur que EF se chargera de remplir ces données
        [ForeignKey("IdAnnonce")]
        public virtual Annonce Annonce { get; set; } = null!;

        [ForeignKey("IdAttribut")]
        public virtual Attribut Attribut { get; set; } = null!;
    }
}