using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Interfaces;

public interface IEmailManager
{
    Task SendEmailAsync(string toEmail, string subject, string message, List<IFormFile> attachments = null);
    Task SendEmailAsync(string toEmail, string subject, string message);

}
