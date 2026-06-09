using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Infrastructure.Configuration;
using YMMO.Backend.Infrastructure.Data.Configurations;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("YMMO Support", _settings.SenderEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, false);
        await client.AuthenticateAsync(_settings.User, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    
    public async Task SendOfferNotificationAsync(string to, string clientName, Guid propertyId, string status)
    {
        string subject = "Mise à jour de votre Projet Immobilier";
        string body = $"Bonjour {clientName},\n\nL'offre pour votre bien (ID: {propertyId}) est désormais : {status}.";
    
        await SendEmailAsync(to, subject, body);
    }
}