using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Ecommerce.Application.Dto;

public class ProductForEditDto
{
    public long ProductId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public string? Name { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public string? Slug { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public string? ShortDescription { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_\[\]""-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-', '_', brackets, and quotes are allowed.")]
    public string? KeySpecs { get; set; }

    public List<string> KeySpecsList => JsonSerializer.Deserialize<List<string>>(KeySpecs ?? "[]");

    public string? Description { get; set; }
    public string? VariableTheme { get; set; }
    public int CategoryId { get; set; }
    public string? ProductImage { get; set; }
    public string? ProductImagePreview { get; set; }
    public List<ProductVariantForEditDto> ProductVariant { get; set; } = new List<ProductVariantForEditDto>();
}

public class ProductVariantForEditDto
{
    public long Id { get; set; }
    public string? CategoryName { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public string? Title { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public string? Slug { get; set; }

    public long ProductId { get; set; }
    public int? SizeId { get; set; }
    public int? ColorId { get; set; }
    public string? VariantImageId { get; set; }
    public string? VariantImagePreview { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public string? Sku { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public int Qty { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?#_-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '#', '-' and '_' are allowed.")]
    public decimal Price { get; set; }
}
