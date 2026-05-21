using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        // 1. Vérifier si l'email existe
        if (await _userRepository.ExistsAsync(request.Email))
            throw new Exception("Cet email est déjà utilisé.");

        // 2. Générer le code de vérification
        var verificationCode = new Random().Next(100000, 999999).ToString();

        // 3. Créer l'entité User
        var user = new User
        {
            Nom = request.Nom,
            Prenom = request.Prenom,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Telephone = request.Telephone,
            RoleId = 2, // Rôle utilisateur par défaut
            VerificationCode = verificationCode,
            CodeExpiresAt = DateTime.UtcNow.AddHours(24),
            IsVerified = false,
            DateInscription = DateTime.UtcNow,
            EstActif = true

        };

        // 4. Sauvegarder
        await _userRepository.AddAsync(user);

        // 5. Envoyer l'email
        await _emailService.SendVerificationEmailAsync(user.Email, verificationCode);

        return "Inscription réussie. Veuillez vérifier votre boîte mail.";
    }

    public async Task<bool> VerifyEmailAsync(string email, string code)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || user.VerificationCode != code || user.CodeExpiresAt < DateTime.UtcNow)
            return false;

        user.IsVerified = true;
        user.VerificationCode = null;
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<TokenRespnse> LoginAsync(LoginRequest request )
    {
        // 1. Récupérer l'utilisateur
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new Exception("Email ou mot de passe invalide.");

        // 2. Vérifier le mot de passe
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Email ou mot de passe invalide.");

        // 3. Vérifier si l'email est vérifié
        if (!user.IsVerified)
            throw new Exception("Veuillez vérifier votre email avant de vous connecter.");

        // 4. Vérifier si le compte est actif
       // if (!user.EstActif)
        //    throw new Exception("Votre compte a été désactivé.");

        // 5. Mettre à jour la dernière connexion
        user.DerniereConnexion = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // 6. Générer et retourner le token JWT
        var token = GenerateJwtToken(user);
        // 7. Générer et stocker le refresh token
        var refreshToken = GenerateRefreshToken();
        user.RefreshTokenHash  = BCrypt.Net.BCrypt.HashPassword(refreshToken); ;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // Par exemple, 7 jours
        user.RefreshTokenRevokedAt = null;
        await _userRepository.UpdateAsync(user);

        return new TokenRespnse
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }

    private string GenerateJwtToken(User user)
    {
        // Créer les claims (informations encodées dans le token)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}"),
            new Claim(ClaimTypes.GivenName, user.Prenom),
            new Claim(ClaimTypes.Surname, user.Nom),
            new Claim("RoleId", user.RoleId.ToString()),
            new Claim(ClaimTypes.Role, user.RoleId == 2 ? "Admin" : "User"),
        };

        // Récupérer la clé secrète depuis appsettings.json
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

        // Définir les credentials de signature
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Récupérer la durée d'expiration depuis la configuration
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "30");

        // Créer le token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds
        );

        // Retourner le token sous forme de chaîne
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

   

   

    public async Task<TokenRespnse> RefreshAsync(RefreshRequest refreshRequest)
    {
        var user = await _userRepository.GetByEmailAsync(refreshRequest.Email);
        if (user == null)
            throw new Exception("Utilisateur non trouvé");

        if (user.RefreshTokenRevokedAt != null)
            throw new Exception("Refresh token révoqué");

        if (user.RefreshTokenExpiresAt == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            throw new Exception("Refresh token expiré");

        bool refreshValid = BCrypt.Net.BCrypt.Verify(refreshRequest.RefreshToken, user.RefreshTokenHash);
        if (!refreshValid)
            throw new Exception("Refresh token invalide");

        // Générer un nouveau token d'accès
        var newAccessToken = GenerateJwtToken(user);

        // Rotation: remplacer le refresh token
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        user.RefreshTokenRevokedAt = null;
        await _userRepository.UpdateAsync(user);

        return new TokenRespnse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
    public async Task LogoutAsync(LogoutRequest logoutRequest)
    {
        var user = await _userRepository.GetByEmailAsync(logoutRequest.Email);
        if (user == null)
            throw new Exception("Utilisateur non trouvé");
        // Révoquer le refresh token
        user.RefreshTokenRevokedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }
}