namespace Ecommerce.Application.Dto;
public class SmtpConfigurationDto
{
    public string? EmailFromName { get; set; }
    public string? EmailFromEmail { get; set; }
    public string? EmailUserName { get; set; }
    public string? EmailPassword { get; set; }
    public string? EmailHost { get; set; }
    public string? EmailPort { get; set; }
}