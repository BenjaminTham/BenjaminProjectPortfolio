using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;
public class HeaderSliderDto
{
    public string? Image { get; set; }
    public string? ImagePreview { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_$-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', '-', and '$' are allowed.")]
    public string? HeaderTextLineOne { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_$-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', '-', and '$' are allowed.")]
    public string? HeaderTextLineTwo { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_$-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', '-', and '$' are allowed.")]
    public string? SubText { get; set; }

    public bool IsActive { get; set; }
    public int Order { get; set; }
}