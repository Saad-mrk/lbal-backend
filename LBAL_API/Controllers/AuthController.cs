using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Application.DTOs.Auth;
using LBAL.API.Common.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace LBALAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService , ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
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
    [EnableRateLimiting("AuthLimiter")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        try
        {
            var tokenResponse = await _authService.LoginAsync(request);
            return Ok(ApiResponse<TokenRespnse>.SuccessResponse(tokenResponse, "Connexion réussie."));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
          "Failed login attempt (email not found). Email={Email}, IP={IP}",
          request.Email,
          ip
      );
            return Unauthorized(ApiResponse<string>.ErrorResponse("Échec de l'authentification", ex.Message));
        }
    }

 

    /// <summary>
    /// Récupère les informations de l'utilisateur connecté via le JWT
    /// </summary>
   


    [EnableRateLimiting("AuthLimiter")]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
    {
        try
        {
            var tokenResponse = await _authService.RefreshAsync(refreshRequest);
            return Ok(ApiResponse<TokenRespnse>.SuccessResponse(tokenResponse, "Token rafraîchi avec succès."));
        }
        catch (Exception ex)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Échec du rafraîchissement du token", ex.Message));
        }
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
    {
        try
        {
            await _authService.LogoutAsync(logoutRequest);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Déconnexion réussie."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("Erreur lors de la déconnexion", ex.Message));
        }
    }

}