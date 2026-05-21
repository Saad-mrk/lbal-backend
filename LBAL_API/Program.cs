using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Data;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting; // Ajouté pour IWebHostEnvironment

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURATION DES SERVICES (DEPENDENCY INJECTION)
// ============================================================

// --- Base de Données ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Couche Infrastructure ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICategorieRepository, CategorieRepository>();
builder.Services.AddScoped<IAnnonceRepository, AnnonceRepository>();
builder.Services.AddScoped<IFileService, FileService>();

// --- Couche Application ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnnonceService, AnnonceService>();
builder.Services.AddScoped<CategorieService>(); // Injection directe

// --- Configuration de l'environnement pour les fichiers ---
// CORRECTION IMPORTANTE : Configurez explicitement le chemin wwwroot
builder.Services.Configure<HostOptions>(options => { });
builder.WebHost.UseWebRoot("wwwroot"); // Force l'utilisation du dossier wwwroot

// --- Authentification JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "CleDeSecoursDePlusDe32CaracteresPourEviterLeCrash")),
        ClockSkew = TimeSpan.Zero
    };
});

// --- Autorisation & Rate Limiting ---
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1)
        });
    });
});

// --- Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // FIX ERREUR 500 : Cette ligne permet de gérer les classes qui ont le même nom 
    // dans des namespaces différents (ex: Application.DTOs.Annonce et LBAL.DTOs.Annonce)
    options.CustomSchemaIds(type => type.FullName);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT : Bearer {votre_token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================
// 2. PIPELINE DE REQUÊTE (MIDDLEWARES)
// ============================================================

var app = builder.Build();

// CORRECTION : Créez le dossier wwwroot et les sous-dossiers AVANT d'exécuter l'application
var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
var uploadsPath = Path.Combine(webRootPath, "uploads", "annonces");

// Créez les dossiers s'ils n'existent pas
if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);
    app.Logger.LogInformation("Dossier wwwroot créé à : {Path}", webRootPath);
}

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
    app.Logger.LogInformation("Dossier des uploads créé à : {Path}", uploadsPath);
}

// Toujours afficher Swagger en développement pour tester
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LBAL API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // IMPORTANT : Permet de servir les images uploadées

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Middleware de Log pour les erreurs 403
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        app.Logger.LogWarning("Accès refusé ! UserId: {UserId}, Path: {Path}", userId, context.Request.Path);
    }
});

app.MapControllers();

app.Run();