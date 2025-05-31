using Ecommerce.Application.Models;

namespace Ecommerce.Application.Dto;

public class RptOrderInvoiceDto
{
    public string InvoiceNo { get; set; }
    public List<RptOrderInvoicePageInfo> ReportPageInfos { get; set; }
    public List<ReportParameter> CustomerInfo { get; set; }
    public List<ReportParameter> OrderSummary { get; set; }
    public List<RptOrderInvoiceOrderItems> OrderItems { get; set; }

    public List<ReportParameter> OrderAmount { get; set; }
}

public class RptOrderInvoiceOrderItems
{
    public long OrderId { get; set; }
    public string Item { get; set; }
    public decimal UnitPrice { get; set; }
    public string UnitPriceWithCurrency { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public string TotalWithCurrency { get; set; }
}

public class RptOrderInvoicePageInfo
{
    public string CompanyName { get; set; }
}
