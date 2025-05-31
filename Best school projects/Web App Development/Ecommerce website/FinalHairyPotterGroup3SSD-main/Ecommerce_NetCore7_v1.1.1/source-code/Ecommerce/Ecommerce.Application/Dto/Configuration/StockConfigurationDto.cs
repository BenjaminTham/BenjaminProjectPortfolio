namespace Ecommerce.Application.Dto;
public class StockConfigurationDto
{
    public bool IsLowStockNotificationEnabled { get; set; } = false;
    public bool IsOutOfStockNotificationEnabled { get; set; } = false;
    public int? LowStockThreshold { get; set; } = 0;
    public int? OutOfStockThreshold { get; set; } = 0;
    public bool IsOutOfStockItemHidden { get; set; } = false;
}