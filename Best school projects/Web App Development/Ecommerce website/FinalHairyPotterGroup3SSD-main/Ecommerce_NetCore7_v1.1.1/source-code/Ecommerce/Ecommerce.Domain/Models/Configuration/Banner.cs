using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Models;
public class Banner
{
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? ColorHexCode { get; set; }
    public string? BackgroundColorHexCode { get; set; }
    public string? BannerType { get; set; }
    public bool IsActive { get; set; }
}