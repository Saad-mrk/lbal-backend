using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Interfaces;
namespace Application.Services;

    public class CategorieService
    {
        private ICategorieRepository _categorieRepository;
    // Exemple de méthode dans ton service/QueryHandler de la couche Application
    public CategorieService(ICategorieRepository categorieRepository)
    {
        _categorieRepository = categorieRepository;
    }
    public async Task<List<CategorieDto>> GetCategoriesTreeAsync()
    {
        // On récupère les catégories depuis le repository ou le DbContext en incluant les enfants
        var categories = await _categorieRepository.GetAllWithSousCategoriesAsync();

        // Mapping des Entités vers les DTOs pour structurer le JSON pour le Front
        return categories.Select(c => new CategorieDto
        {
            Id = c.Id,
            Nom = c.Nom,
            Children = c.SousCategories.Select(sc => new SousCategorieDto
            {
                Id = sc.Id,
                Nom = sc.Nom
            }).ToList()
        }).ToList();
    }
}

