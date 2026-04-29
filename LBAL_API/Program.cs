using System;
using LBAL.Application.Interfaces;
using LBAL.Application.Services;
using LBAL.Domain.Interfaces;
using LBAL.Infrastructure.Data;
using LBAL.Infrastructure.Repositories;
using LBAL.Infrastructure.Security;
using LBAL.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuration de la Base de Données ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. Injection des dépendances (Dependency Injection) ---

// Couche Infrastructure
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Couche Application
builder.Services.AddScoped<IAuthService, AuthService>();

// --- 3. Services standards API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ajouter l'authentification (si vous utilisez session/cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 4. CORS (Optionnel mais recommandé pour React) ---
// À ajouter si ton frontend React est sur un autre port (ex: 5173 ou 3000)
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();