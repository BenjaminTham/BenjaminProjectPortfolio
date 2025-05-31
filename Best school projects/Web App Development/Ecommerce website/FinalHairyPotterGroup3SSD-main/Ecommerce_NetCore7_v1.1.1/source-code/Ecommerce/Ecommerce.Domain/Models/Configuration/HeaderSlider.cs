namespace Ecommerce.Domain.Models;

public class HeaderSlider
{
    public string? Image { get; set; }
    public string? HeaderTextLineOne { get; set; }
    public string? HeaderTextLineTwo { get; set; }
    public string? SubText { get; set; }
    public bool IsActive { get; set; } = false;
    public int Order { get; set; }
}

