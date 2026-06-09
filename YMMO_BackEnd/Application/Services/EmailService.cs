using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // TODO: Implement SMTP client or API call to email provider
        await Task.CompletedTask;
    }

    public async Task SendOfferNotificationAsync(string to, string clientName, Guid propertyId, string status)
    {
        string subject = "Mise à jour de votre Projet Immobilier";
        string body = $"Bonjour {clientName},\n l'offre pour votre bien {propertyId} est désormais {status}.";
        
        await SendEmailAsync(to, subject, body);
    }
}