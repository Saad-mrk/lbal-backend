using MailKit.Net.Smtp;
using MimeKit;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendVerificationEmailAsync(string email, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("LBAL App", _config["EmailSettings:From"]));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Vérification de votre compte";

        message.Body = new TextPart("html")
        {
            Text = $@"<h1>Bienvenue !</h1>
                      <p>Votre code de vérification est : <b>{code}</b></p>
                      <p>Ce code expirera dans 24 heures.</p>"
        };

        using var client = new SmtpClient();
        // Connexion au serveur SMTP (ex: Gmail)
        await client.ConnectAsync(_config["EmailSettings:Host"], 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}