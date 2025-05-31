using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dto;
public class OrderDetailsDto
{
    public int Id { get; set; }
    public long OrderId { get; set; }
    public string ProductName { get; set; }
    //public long ProductVariantId { get; set; }
    //public string? VariantImagePreview { get; set; }
    public decimal UnitPrice { get; set; }
    public int Qty { get; set; }
}
