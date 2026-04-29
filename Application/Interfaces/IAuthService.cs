using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LBAL.Application.DTOs;
using LBAL.Domain.Interfaces;

namespace LBAL.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<bool> VerifyEmailAsync(string email, string code);
    Task<string> LoginAsync(LoginRequest request); // Retournera le JWT
    Task UpdateLastConnectionAsync(int userId); // Nouvelle méthode pour mettre à jour la dernière connexion
      
    
}