using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger; // Ajouté pour les logs

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
        {
            try
            {
                // Étape 1: Vérifiez et corrigez WebRootPath s'il est null
                string webRootPath = _environment.WebRootPath;

                _logger.LogInformation("WebRootPath original: {WebRootPath}", webRootPath);

                if (string.IsNullOrEmpty(webRootPath))
                {
                    // Si WebRootPath est null, utilisez ContentRootPath + "wwwroot"
                    webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                    _logger.LogInformation("WebRootPath corrigé: {WebRootPath}", webRootPath);

                    // Créez le dossier wwwroot s'il n'existe pas
                    if (!Directory.Exists(webRootPath))
                    {
                        Directory.CreateDirectory(webRootPath);
                        _logger.LogInformation("Dossier wwwroot créé");
                    }
                }

                // Étape 2: Construisez le chemin complet
                var rootPath = Path.Combine(webRootPath, subDirectory);
                _logger.LogInformation("Chemin complet: {RootPath}", rootPath);

                // Étape 3: Créez le dossier s'il n'existe pas
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                    _logger.LogInformation("Dossier créé: {RootPath}", rootPath);
                }

                // Étape 4: Générez un nom de fichier unique
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(rootPath, fileName);
                _logger.LogInformation("Chemin du fichier: {FilePath}", filePath);

                // Étape 5: Sauvegardez le fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Étape 6: Retournez l'URL relative
                var relativeUrl = $"/{subDirectory}/{fileName}";
                _logger.LogInformation("URL retournée: {Url}", relativeUrl);

                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde du fichier {FileName}", file.FileName);
                throw new Exception($"Erreur lors de la sauvegarde du fichier {file.FileName}: {ex.Message}", ex);
            }
        }
    }
}