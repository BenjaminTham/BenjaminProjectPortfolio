using Ecommerce.Application.Helpers;

namespace Ecommerce.Application.Dto;

public class ShopDto
{
    public PaginatedList<ShopShowcaseDto>? PaginatedProductList { get; set; }
}
