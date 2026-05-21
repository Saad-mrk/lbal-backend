using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class AnnonceCreateDto
    {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int IdCategorie { get; set; }
        public int IdSousCategorie { get; set; }
        public string Etat { get; set; } = string.Empty; 
        public string Statut { get; set; } = string.Empty;

        // Utilisation de IFormFile pour recevoir les fichiers via l'API
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        // Liste d'attributs (ex: [{"IdAttribut": 1, "Valeur": "XL"}])
        public List<AnnonceAttributDto> Attributs { get; set; } = new List<AnnonceAttributDto>();
    }

    public class AnnonceAttributDto
    {
        public int IdAttribut { get; set; }
        public string Valeur { get; set; } = string.Empty;
    }
}