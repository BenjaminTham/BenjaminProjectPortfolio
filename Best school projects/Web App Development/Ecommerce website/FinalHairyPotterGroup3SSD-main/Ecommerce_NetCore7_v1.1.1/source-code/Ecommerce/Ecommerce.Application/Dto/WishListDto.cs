using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dto;
public class WishListDto
{
    public long ProductId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Price { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? CategoryId { get; set; }
    public string? CategorySlug { get; set; }
    public string? CategoryName { get; set; }
    public string? ProductImage { get; set; }
    public string? ProductImagePreview { get; set; }
}
