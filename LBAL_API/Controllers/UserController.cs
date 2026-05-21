using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using LBAL.API.Common.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LBAL.API.Controllers;

[Authorize] // Protège tout le contrôleur par défaut
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var user = await _userService.GetUserByIdAsync(userId);
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Profil récupéré."));
    }

    [HttpGet] // GET api/users?searchTerm=...&role=...
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDto filter)
    {
        var users = await _userService.GetUsersAsync(filter);
        return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users, "Liste récupérée."));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound(ApiResponse<UserDto>.ErrorResponse("Introuvable", "L'utilisateur n'existe pas."));
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Utilisateur trouvé."));
    }

    [HttpPost("activity")]
    public async Task<IActionResult> UpdateActivity()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await _userService.UpdateLastConnectionAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Activité mise à jour."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserDto userDto)
    {
        // Sécurité : Un utilisateur ne peut modifier que son propre profil (sauf s'il est Admin)
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        if (currentUserId != id && !User.IsInRole("Admin"))
            return Forbid();

        var success = await _userService.UpdateUserAsync(id, userDto);
        return success ? Ok(ApiResponse<string>.SuccessResponse("Mise à jour réussie")) : BadRequest();
    }
}