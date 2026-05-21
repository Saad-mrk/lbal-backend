using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<bool> VerifyEmailAsync(string email, string code);
    Task<TokenRespnse> LoginAsync(LoginRequest request); // Retournera le JWT
    Task<TokenRespnse> RefreshAsync(RefreshRequest refreshRequest);
    Task LogoutAsync(LogoutRequest logoutRequest);

}