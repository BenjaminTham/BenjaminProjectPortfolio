namespace Ecommerce.Application.Dto;
public class DashboardDto
{
    public int PendingOrderCount { get; set; }
    public int LowStockItemCount { get; set; }
    public TodaySalesSummary TodaySalesSummary { get; set; }
    public CustomerInfo CustomerInfo { get; set; }
    public WeeklyItemSalesCount WeeklyItemSalesCount { get; set; }
}

public class WeeklySalesCount
{
    public DateTime OrderDate { get; set; }
    public string NameOfDay { get; set; }
    public int Count { get; set; }
}

public class WeeklyItemSalesCount
{
    public DateTime OrderDate { get; set; }
    public string NameOfDay { get; set; }
    public int ItemCount { get; set; }
    public int ProductCount { get; set; }
}

public class CustomerInfo
{
    public int TotalCustomer { get; set; }
    public int TotalCustomerToday { get; set; }
}

public class TodaySalesSummary
{
    public int? TotalProduct { get; set; }
    public int? TotalItem { get; set; }
    public int? TotalSales { get; set; }
    public decimal? TotalSalesAmount { get; set; }
    public decimal? TotalDeliveryCharge { get; set; }
}


public class WeeklySalesAmount
{
    public DateTime OrderDate { get; set; }
    public string NameOfDay { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal DeliveryCharge { get; set; }
}