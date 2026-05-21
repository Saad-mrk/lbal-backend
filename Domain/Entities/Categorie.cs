using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("CATEGORIE", Schema = "dbo")] // On cible ta table exacte

    public class Categorie
    {
        [Key]
        [Column("id_categorie")]
        public int Id { get; set; }
        [Column("nom")]

        public string Nom { get; set; }

        // Propriété de navigation pour la relation 1-à-plusieurs
        public virtual ICollection<SousCategorie> SousCategories { get; set; } = new List<SousCategorie>();
    }
    [Table("SOUS_CATEGORIE", Schema = "dbo")]

    public class SousCategorie
    {
        [Key]
        [Column("id_sous_categorie")]
        public int Id { get; set; }
        [Column("nom")]

        public string Nom { get; set; }
        [Column("id_categorie")]

        public int CategorieId { get; set; }

        // Propriété de navigation inverse
        public virtual Categorie Categorie { get; set; }
    }
}
