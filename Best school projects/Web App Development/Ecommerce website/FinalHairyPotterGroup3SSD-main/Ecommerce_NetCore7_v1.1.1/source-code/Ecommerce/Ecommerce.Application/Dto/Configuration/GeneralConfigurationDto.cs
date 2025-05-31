using Ecommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;
public class GeneralConfigurationDto
{

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', and '-' are allowed.")]
    public string? CompanyName { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', and '-' are allowed.")]
    public string? CompanySlogan { get; set; }

    public string? CompanyLogo { get; set; }
    public string? CompanyLogoPreview { get; set; }
    public string? CompanyFavicon { get; set; }
    public string? CompanyFaviconPreview { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string? CompanyContact { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string? CompanyEmail { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '_' are allowed.")]
    public string? CompanyLocation { get; set; }

    [RegularExpression(@"^[\p{Sc}]*$", ErrorMessage = "Only currency symbols are allowed.")]
    public string? CurrencySymbol { get; set; }

    public CurrencyPosition? CurrencyPosition { get; set; }
    public bool IsPaypalEnabled { get; set; }
    public bool IsStripeEnabled { get; set; }
}