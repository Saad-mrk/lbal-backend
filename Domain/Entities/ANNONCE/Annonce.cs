using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("ANNONCE")]
    public class Annonce
    {
        [Key]
        [Column("id_annonce")]  // ✅ Espace supprimé
        public int IdAnnonce { get; set; }

        [Column("id_utilisateur")]
        public int id_utilisateur { get; set; }
        [ForeignKey("id_utilisateur")]
        public virtual User User { get; set; } = null!;

        [Column("id_categorie")]
        public int IdCategorie { get; set; }

        [Column("id_sous_categorie")]
        public int IdSousCategorie { get; set; }

        [Column("titre")]
        public string Titre { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("prix")]
        public decimal Prix { get; set; }

        [Column("etat_id")]
        public int EtatId { get; set; }

        [Column("statut_id")]
        public int StatutId { get; set; }

        [Column("localisation_ville")]
        public string LocalisationVille { get; set; } = string.Empty;

        [Column("slug")]
        public string? Slug { get; set; }

        [Column("nombre_vues")]
        public int NombreVues { get; set; } = 0;

        [Column("date_publication")]
        public DateTime DatePublication { get; set; } = DateTime.Now;



        // Relations (Navigation Properties)


        [ForeignKey("IdCategorie")]
        public virtual Categorie Categorie { get; set; } = null!;

        [ForeignKey("IdSousCategorie")]
        public virtual SousCategorie SousCategorie { get; set; } = null!;


        public virtual ICollection<PhotoAnnonce> Photos { get; set; } = new List<PhotoAnnonce>();
        public virtual ICollection<AnnonceAttribut> Attributs { get; set; } = new List<AnnonceAttribut>();

    }
}