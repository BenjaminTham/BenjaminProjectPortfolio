using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Dto.Stock;

public class LowStockProductDto
{
    public long ProductId { get; set; }
    public long VariantId { get; set; }
    public string Category { get; set; }
    public string ProductTitle { get; set; }
    public string VariantTitle { get; set; }
    public int Qty { get; set; }
    public int TotalVariant { get; set; }
    public int LowStockVariantCount { get; set; }
}

