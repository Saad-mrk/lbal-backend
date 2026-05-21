using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LBAL_API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AnnonceController : ControllerBase
    {
        private readonly IAnnonceService _annonceService;

        public AnnonceController(IAnnonceService annonceService)
        {
            _annonceService = annonceService;
        }

        [HttpPost]
        [Authorize]

        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] AnnonceCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);


                if (userId <= 0)
                {
                    return Unauthorized("Vous devez être connecté");
                }


                if (dto.Images == null || dto.Images.Count == 0)
                {
                    return BadRequest("Au moins une image est requise");
                }

                var annonceId = await _annonceService.CreateAnnonceAsync(dto, userId);

                return Ok(new { id = annonceId, message = "Annonce créée avec succès" });
            }
            catch (Exception ex)
            {
                // AFFICHEZ L'ERREUR COMPLÈTE
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" | Inner: {ex.InnerException.Message}";
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage += $" | Inner Inner: {ex.InnerException.InnerException.Message}";
                    }
                }

                return StatusCode(500, $"Erreur interne : {errorMessage}");
            }
        }
        [HttpGet("me/statut/{statut}")]
        [Authorize]
        public async Task<IActionResult> GetByUserId( string statut)
        {
            try

            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                var annonces = await _annonceService.GetAnnoncesByUserIdAsync(statut ,userId);
                if (annonces == null || !annonces.Any())
                {
                    return NotFound("Aucune annonce trouvée pour cet utilisateur et ce statut");
                }
                return Ok(new { data = annonces, message = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

    }
}