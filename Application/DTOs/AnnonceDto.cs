
using Domain.Entities;

namespace Application.DTOs
{
    public class AnnonceDto
    {
        public int  Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public string SousCategorie { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public string etat { get; set; } = string.Empty;

        // On renvoie maintenant TOUTES les URLs des photos
        public List<string> PhotosUrls { get; set; } = new List<string>();

        // Tes attributs spécifiques
        public string Brand { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime datepublication { get; set; }

    }
}
