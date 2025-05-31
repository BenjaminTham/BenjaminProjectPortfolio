using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Enums;
public enum StockInputType
{
    [Display(Name = "Stock Added")]
    Addition = 1,
    [Display(Name = "Stock Deducted")]
    Deduction = 2
}