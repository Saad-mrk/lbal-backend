using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    // Dans le projet 'Application' (Catégorie avec ses enfants intégrés)

    public class CategorieDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public List<SousCategorieDto> Children { get; set; } = new List<SousCategorieDto>();
    }



    public class SousCategorieDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
    }
}
