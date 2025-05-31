using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dto;
public class BannerDto
{
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_$%-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', '-', and '$' are allowed.")]
    public string? Title { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_$%-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', '-', and '$' are allowed.")]
    public string? SubTitle { get; set; }

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Invalid color hex code.")]
    public string? ColorHexCode { get; set; }

    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Invalid background color hex code.")]
    public string? BackgroundColorHexCode { get; set; }
    public string? BannerType { get; set; }
    public bool IsActive { get; set; }
}