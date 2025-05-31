using Ecommerce.Application.Dto.Shop;

namespace Ecommerce.Application.Dto;

public class ShopDetailsDto
{
    public ProductShopItemsDto ProductDetails { get; set; } = new ProductShopItemsDto();
    public List<ColorDto> AvailableColors { get; set; } = new List<ColorDto>();
    public List<SizeDto> AvailableSizes { get; set; } = new List<SizeDto>();
    public List<GalleryPreviewDto> AvailableImages { get; set; } = new List<GalleryPreviewDto>();
    public List<ColorSizeDto> ColorSizeCombination { get; set; } = new List<ColorSizeDto>();
}


public class GalleryPreviewDto
{
    public string? Id { get; set; }
    public string? Url { get; set; }
}