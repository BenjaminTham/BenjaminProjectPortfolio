using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Models;
public class GeneralConfiguration
{
    public string? CompanyName { get; set; }
    public string? CompanySlogan { get; set; }
    public string? CompanyLogo { get; set; }
    public string? CompanyFavicon { get; set; }
    public string? CompanyContact { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyLocation { get; set; }
    public string? CurrencySymbol { get; set; }
    public CurrencyPosition? CurrencyPosition { get; set; }
    public bool IsPaypalEnabled { get; set; } = true;
    public bool IsStripeEnabled { get; set; } = true;
}

