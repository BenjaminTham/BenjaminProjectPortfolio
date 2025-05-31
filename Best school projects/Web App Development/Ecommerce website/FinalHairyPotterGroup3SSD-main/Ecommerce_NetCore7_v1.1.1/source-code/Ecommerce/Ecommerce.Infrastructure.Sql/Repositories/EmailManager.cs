using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System.Text.Json;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Ecommerce.Infrastructure.Sql.Repositories;

public class EmailManager : IEmailManager
{
    public IKeyAccessor _keyAccessor { get; set; }
    public EmailManager(IKeyAccessor keyAccessor)
    {
        _keyAccessor = keyAccessor;
    }

    public Task SendEmailAsync(string toEmail, string subject, string message, List<IFormFile> attachments)
    {
        return Execute(toEmail, subject, message, attachments);
    }


    public Task SendEmailAsync(string toEmail, string subject, string message)
    {
        return Execute1(toEmail, subject, message);
    }

    public Task Execute1(string to, string subject, string message)
    {
        var smtpConfig = _keyAccessor["SmtpConfiguration"] != null ? JsonSerializer.Deserialize<SmtpConfigurationDto>(_keyAccessor["SmtpConfiguration"]) : new SmtpConfigurationDto();

        // create message
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(smtpConfig?.EmailFromEmail);
        if (!string.IsNullOrEmpty(smtpConfig?.EmailFromName))
            email.Sender.Name = smtpConfig?.EmailFromName;
        email.From.Add(email.Sender);
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = message;
        email.Body = builder.ToMessageBody();

        // send email
        using (var smtp = new SmtpClient())
        {
            smtp.Connect(smtpConfig?.EmailHost, Int32.Parse(smtpConfig?.EmailPort), SecureSocketOptions.StartTls);
            smtp.Authenticate(smtpConfig?.EmailUserName, smtpConfig?.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        return Task.FromResult(true);
    }

    public Task Execute(string to, string subject, string message, List<IFormFile> attachments)
    {
        var smtpConfig = _keyAccessor["SmtpConfiguration"] != null ? JsonSerializer.Deserialize<SmtpConfigurationDto>(_keyAccessor["SmtpConfiguration"]) : new SmtpConfigurationDto();

        // create message
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(smtpConfig?.EmailFromEmail);
        if (!string.IsNullOrEmpty(smtpConfig?.EmailFromName))
            email.Sender.Name = smtpConfig?.EmailFromName;
        email.From.Add(email.Sender);
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder();
        if (attachments != null)
        {
            byte[] fileBytes;
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody = message;
        email.Body = builder.ToMessageBody();

        // send email
        using (var smtp = new SmtpClient())
        {
            smtp.Connect(smtpConfig?.EmailHost, Int32.Parse(smtpConfig?.EmailPort), SecureSocketOptions.StartTls);
            smtp.Authenticate(smtpConfig?.EmailUserName, smtpConfig?.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        return Task.FromResult(true);
    }
}
