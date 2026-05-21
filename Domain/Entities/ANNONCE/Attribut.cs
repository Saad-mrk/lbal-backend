using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
        [Table("ATTRIBUT")]
    public class Attribut
    {
        [Key]
        [Column("id_attribut ")]
        public int IdAttribut { get; set; }
        [Column ("nom")]
        [Required]
        public string Nom { get; set; } = string.Empty; // ex: "Taille", "Couleur", "Kilométrage"
    }
}
