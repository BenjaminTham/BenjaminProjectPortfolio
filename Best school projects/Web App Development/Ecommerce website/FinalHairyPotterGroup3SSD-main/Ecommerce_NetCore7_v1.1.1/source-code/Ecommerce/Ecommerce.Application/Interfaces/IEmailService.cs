using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string message, List<IFormFile> attachments);
    Task SendTwoFactorEmailAsync(string username, string subject);
    Task SendResetPasswordEmailAsync(string email, string subject, string url);
    Task SendEmailConfirmationAsync(string email, string subject, string url);
}
