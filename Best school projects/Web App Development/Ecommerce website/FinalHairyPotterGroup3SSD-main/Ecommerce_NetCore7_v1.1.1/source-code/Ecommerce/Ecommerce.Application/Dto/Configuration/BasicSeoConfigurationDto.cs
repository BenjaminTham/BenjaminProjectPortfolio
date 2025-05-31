using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;
public class BasicSeoConfigurationDto
{
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '#' are allowed in the shipping address.")]
    public string? SeoTitle { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '#' are allowed in the shipping address.")]
    public string? SeoDescription { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '#' are allowed in the shipping address.")]
    public string? SeoKeywords { get; set; }
}