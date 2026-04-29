using LBAL.Domain.Entities;
using LBAL.Domain.Interfaces;
using LBAL.Application.Interfaces;
using LBAL.Application.DTOs;

namespace LBAL.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        // 1. Vérifier si l'email existe
        if (await _userRepository.ExistsAsync(request.Email))
            throw new Exception("Cet email est déjà utilisé.");

        // 2. Générer le code de vérification
        var verificationCode = new Random().Next(100000, 999999).ToString();

        // 3. Créer l'entité User (Assure-toi que les noms correspondent à User.cs)
        var user = new User
        {
            Nom = request.Nom,
            Prenom = request.Prenom,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Telephone = request.Telephone,

            // Attention : Utilise le nom exact défini dans ton fichier User.cs
            // Si tu as mis [Column("role_id")] public int RoleId... alors utilise RoleId
            RoleId = 2,

            // Ces champs vont maintenant être sauvegardés en base !
            VerificationCode = verificationCode,
            CodeExpiresAt = DateTime.UtcNow.AddHours(24),
            IsVerified = false,

            // Champs additionnels vus dans ta DB
            DateInscription = DateTime.UtcNow,
            EstActif = true
        };

        // 4. Sauvegarder (Ici, EF va maintenant écrire le verification_code en SQL)
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
        user.VerificationCode = null; // On nettoie le code
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // 1. Vérification existance
        if (user == null) throw new Exception("Utilisateur non trouvé.");

        // 2. Vérification Password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Identifiants invalides.");

        // 3. LA MISE À JOUR (À faire ici !)
        user.DerniereConnexion = DateTime.Now;
        await _userRepository.UpdateAsync(user); // On force l'écriture en SQL

        // 4. Pour l'instant tu retournes une string simple
        return "Connexion réussie sans JWT pour le moment";
    }
    // AuthService.cs
    public async Task UpdateLastConnectionAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.DerniereConnexion = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
    }
}