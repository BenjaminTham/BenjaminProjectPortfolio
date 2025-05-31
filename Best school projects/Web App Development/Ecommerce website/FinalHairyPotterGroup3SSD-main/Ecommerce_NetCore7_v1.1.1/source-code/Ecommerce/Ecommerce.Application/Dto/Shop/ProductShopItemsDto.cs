using System.Text.Json;

namespace Ecommerce.Application.Dto;

public class ProductShopItemsDto
{
    public long Id { get; set; }
    public string? ProductName { get; set; }
    public string? Slug { get; set; }
    public string? KeySpecs { get; set; }
    public List<string> KeySpecsList => JsonSerializer.Deserialize<List<string>>(KeySpecs ?? "[]");
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? VariableTheme { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public long VariantId { get; set; }
    public string? VariantTitle { get; set; }
    public string? Sku { get; set; }
    public int? ColorId { get; set; }
    public string? ColorName { get; set; }
    public string? ColorCode { get; set; }
    public int? SizeId { get; set; }
    public string? SizeName { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public string? ProductImage { get; set; }
    public string? VariantImage { get; set; }
    public int Count { get; set; }
}