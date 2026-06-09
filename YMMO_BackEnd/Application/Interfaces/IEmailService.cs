namespace YMMO.Backend.Application.Interfaces;

public interface IEmailService
{
    // Sends a professional email using a template
    Task SendEmailAsync(string to, string subject, string body);
    
    // Specialized method for notifications (e.g., offer updates)
    Task SendOfferNotificationAsync(string to, string clientName, Guid propertyId, string status);
}