using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging; // Ajoutez ce using

namespace Application.Services
{
    public class AnnonceService : IAnnonceService
    {
        private readonly IAnnonceRepository _repository;
        private readonly IFileService _fileService;
        private readonly ILogger<AnnonceService> _logger; // Ajoutez le logger

        public AnnonceService(IAnnonceRepository repository, IFileService fileService, ILogger<AnnonceService> logger)
        {
            _repository = repository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<int> CreateAnnonceAsync(AnnonceCreateDto dto, int userIdd)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                if (string.IsNullOrWhiteSpace(dto.Titre))
                    throw new ArgumentException("Le titre est requis");

                if (string.IsNullOrWhiteSpace(dto.Description))
                    throw new ArgumentException("La description est requise");

                if (dto.Prix <= 0)
                    throw new ArgumentException("Le prix doit être supérieur à 0");

                if (dto.IdCategorie <= 0)
                    throw new ArgumentException("La catégorie est requise");

                // 1. Création de l'objet Annonce avec initialisation des collections
                var annonce = new Annonce
                {
                    Titre = dto.Titre,
                    Description = dto.Description,
                    Prix = (decimal)(double)dto.Prix,
                    IdCategorie = dto.IdCategorie,
                    IdSousCategorie = dto.IdSousCategorie, // Peut être null si pas de sous-catégorie
                    id_utilisateur = userIdd,
                    StatutId = (int)Enum.Parse(typeof(StatutAnnonce), dto.Statut, ignoreCase: true),
                    EtatId = (int)Enum.Parse(typeof(EtatProduit), dto.Etat, ignoreCase: true),
                    DatePublication = DateTime.UtcNow,
                    Photos = new List<PhotoAnnonce>(),
                    Attributs = new List<AnnonceAttribut>()
                };

                _logger.LogInformation("Annonce créée, titre: {Titre}", annonce.Titre);

                // 2. Gestion des images
                if (dto.Images != null && dto.Images.Any())
                {
                    _logger.LogInformation("Sauvegarde de {Count} images", dto.Images.Count);

                    for (int i = 0; i < dto.Images.Count; i++)
                    {
                        var file = dto.Images[i];
                        if (file == null || file.Length == 0)
                            continue;

                        try
                        {
                            var url = await _fileService.SaveFileAsync(file, "uploads/annonces");

                            var photo = new PhotoAnnonce
                            {
                                Url = url,
                                EstPrincipale = (i == 0) // La première image est principale
                            };

                            annonce.Photos.Add(photo);
                            _logger.LogInformation("Image {Index} sauvegardée: {Url}", i + 1, url);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erreur lors de la sauvegarde de l'image {FileName}", file.FileName);
                            throw new Exception($"Erreur lors de la sauvegarde de l'image {file.FileName}", ex);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Au moins une image est requise");
                }

                // 3. Gestion des attributs (si présents)
                if (dto.Attributs != null && dto.Attributs.Any())
                {
                    _logger.LogInformation("Ajout de {Count} attributs", dto.Attributs.Count);

                    foreach (var attr in dto.Attributs)
                    {
                        if (attr.IdAttribut > 0 && !string.IsNullOrWhiteSpace(attr.Valeur))
                        {
                            annonce.Attributs.Add(new AnnonceAttribut
                            {
                                IdAttribut = attr.IdAttribut,
                                Valeur = attr.Valeur
                            });
                        }
                    }
                }

                // 4. Sauvegarde en base de données
                _logger.LogInformation("Sauvegarde en base de données...");
                await _repository.AddAsync(annonce);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Annonce créée avec succès, ID: {IdAnnonce}", annonce.IdAnnonce);

                return annonce.IdAnnonce;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'annonce pour userId: {UserId}", userIdd);
                throw; // Relance l'exception pour qu'elle soit capturée par le controller
            }
        }
        public async Task<IEnumerable<AnnonceDto>> GetAnnoncesByUserIdAsync(
      string statut,
      int userId)
        {
            try
            {
                var annonces = await _repository.GetById(userId, statut);

                return annonces.Select(a => new AnnonceDto
                {
                    Id = a.IdAnnonce,
                    Titre = a.Titre,
                    Description = a.Description,
                    Prix = a.Prix,
                    Categorie = a.Categorie.Nom,
                    SousCategorie = a.SousCategorie?.Nom,  // ← opérateur null-conditionnel
                    Ville = a.LocalisationVille,
                    etat = ((EtatProduit)a.EtatId).ToString(),

                    PhotosUrls = a.Photos
                        .Select(p => p.Url)
                        .ToList(),

                    // Attributs — corriger "a.anno" → "a.Attributs"
                    Brand = a.Attributs
                        .Where(x => x.IdAttribut == 1)
                        .Select(x => x.Valeur)
                        .FirstOrDefault(),

                    Size = a.Attributs
                        .Where(x => x.IdAttribut == 2)
                        .Select(x => x.Valeur)
                        .FirstOrDefault(),

                    Color = a.Attributs
                        .Where(x => x.IdAttribut == 3)
                        .Select(x => x.Valeur)
                        .FirstOrDefault(),

                    datepublication = a.DatePublication
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des annonces pour userId={UserId}", userId);
                throw;
            }

        }
    }
}