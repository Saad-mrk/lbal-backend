using System.Security.Claims;
using LBAL.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LBAL_API.Middlewares
{
    public class InactivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _inactivityTimeoutMinutes = 15; // 15 minutes

        public InactivityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepo)
        {
            // Vérifie si l'utilisateur est authentifié (peu importe le mécanisme : cookie, session, etc.)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Récupère l'identifiant utilisateur (ajustez selon votre méthode d'authentification)
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    var user = await userRepo.GetByIdAsync(userId);
                    if (user != null)
                    {
                        // Si la dernière activité est trop ancienne
                        if (user.DerniereConnexion < DateTime.UtcNow.AddMinutes(-_inactivityTimeoutMinutes))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Session expirée cause inactivité");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}