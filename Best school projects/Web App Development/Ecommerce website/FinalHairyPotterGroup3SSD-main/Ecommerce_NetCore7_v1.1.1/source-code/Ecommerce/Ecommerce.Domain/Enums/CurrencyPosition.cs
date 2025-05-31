using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Enums;
public enum CurrencyPosition
{
    [Display(Name = "At Start")]
    Start = 1,
    [Display(Name = "At End")]
    End = 2
}