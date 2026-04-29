using System.Security.Claims;
using LBAL.API.Common.Responses; // Import de votre classe ApiResponse
using LBAL.Application.DTOs;
using LBAL.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LBAL.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            // On encapsule la chaîne de caractères (le message de succès)
            return Ok(ApiResponse<string>.SuccessResponse(result, "Inscription réussie."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("Erreur lors de l'inscription", ex.Message));
        }
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string code)
    {
        var isVerified = await _authService.VerifyEmailAsync(email, code);

        if (!isVerified)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Vérification échouée", "Code invalide ou expiré."));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Compte vérifié avec succès !"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request);
            // Ici le Data sera le token
            return Ok(ApiResponse<string>.SuccessResponse(token, "Connexion réussie."));
        }
        catch (Exception ex)
        {
            // On retourne un Unauthorized (401) mais toujours avec le format standard
            return Unauthorized(ApiResponse<string>.ErrorResponse("Échec de l'authentification", ex.Message));
        }
    }
    [Authorize]
    [HttpPost("activity")]
    public async Task<IActionResult> UpdateActivity()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        await _authService.UpdateLastConnectionAsync(userId);
        return Ok();
    }
}