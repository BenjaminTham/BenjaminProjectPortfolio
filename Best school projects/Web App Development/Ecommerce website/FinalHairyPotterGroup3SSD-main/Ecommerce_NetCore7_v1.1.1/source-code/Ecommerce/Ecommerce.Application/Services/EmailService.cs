using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace Ecommerce.Application.Services;

public class EmailService : IEmailService
{
    private readonly IEmailManager _emailManager;
    private readonly IApplicationUserManager _userManager;
    public EmailService(IEmailManager emailManager, IApplicationUserManager userManager)
    {
        _emailManager = emailManager;
        _userManager = userManager;

    }

    public Task SendEmailAsync(string toEmail, string subject, string message, List<IFormFile> attachments)
    {
        return _emailManager.SendEmailAsync(toEmail, subject, message, attachments);
    }

    public Task SendEmailAsync(string toEmail, string subject, string message)
    {
        return _emailManager.SendEmailAsync(toEmail, subject, message);
    }

    public async Task SendResetPasswordEmailAsync(string email, string subject, string url)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = HttpUtility.UrlEncode(token);
        var urlLink = $"{url}?token={token}&email={email}";
        var message = $"<span style='margin-bottom:15.0px'>Please reset your password by clicking here: <br/><a style='font-size:18.0px' href='{url}?token={token}&email={email}'><strong>Reset Password</strong></a></span>";


        var message2 = $"<table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td align='center'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'> <tr> <td align='center' bgcolor='#ffffff' style='padding: 36px 24px 0;'> <h1 style='margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;'>Reset Your Password</h1> </td> </tr> </table> </td> </tr> <tr> <td align='center'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'> <tr> <td align='center' bgcolor='#ffffff' style='padding: 24px; font-size: 16px; line-height: 24px;'> <p style='margin: 0;'>Tap the button below to reset your customer account password. If you didn't request a new password, you can safely delete this email.</p> </td> </tr> <tr> <td align='left' bgcolor='#ffffff'> <table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td align='center' bgcolor='#ffffff' style='padding: 12px;'> <table border='0' cellpadding='0' cellspacing='0'> <tr> <td align='center' bgcolor='#1a82e2' style='border-radius: 6px;'> <a href='{urlLink}' target='_blank' style='display: inline-block; padding: 16px 36px; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;'>Reset Password</a> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table>";
        var res = _emailManager.SendEmailAsync(user.Email, subject, message2);
    }

    public async Task SendTwoFactorEmailAsync(string userName, string subject)
    {
        var user = await _userManager.GetUserByNameAsync(userName);
        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        //var message = "<h1>" + token + "</h1>";
        var message = "<span style='margin-bottom:15.0px'>Your two-factor verification code is: <strong style='color:#ff6600;font-size:24.0px'><br/>" + token + " </strong></span>";
        var res = _emailManager.SendEmailAsync(user.Email, subject, message);
    }

    public async Task SendEmailConfirmationAsync(string userName, string subject, string url)
    {
        var user = await _userManager.GetUserByNameAsync(userName);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        token = HttpUtility.UrlEncode(token);
        var urlLink = $"{url}?token={token}&email={user.Email}";
        var message = $"<span style='margin-bottom:15.0px'>Confirm your account by clicking here: <br/><a style='font-size:18.0px' href='{urlLink}'><strong>Confirm Email</strong></a></span>";
        var res = _emailManager.SendEmailAsync(user.Email, subject, message);
    }



}
