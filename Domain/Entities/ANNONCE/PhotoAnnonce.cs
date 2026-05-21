using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Entities
{
    [Table("PHOTO_ANNONCE")]  // ✅ Sans espace
    public class PhotoAnnonce
    {
        [Key]
        [Column("id_photo")]  // ✅ Sans espace
        public int IdPhoto { get; set; }

        [Column("id_annonce")]  // ✅ Sans espace
        public int IdAnnonce { get; set; }

        [Column("url")]  // ✅ Sans espace
        public string Url { get; set; } = string.Empty;

        [Column("est_principale")]  // ✅ Sans espace
        public bool EstPrincipale { get; set; }

        // Navigation property
        public virtual Annonce Annonce { get; set; }
    }
}