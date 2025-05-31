using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Constants;

public static class DefaultBanner
{
    public static Banner Banner1() { return new Banner { Title = "Get Up To 50% Off", SubTitle = "Keep It Casual", BannerType = BannerType.BannerOne.ToString(), ColorHexCode = "#000000", BackgroundColorHexCode = "#FFD393", IsActive = true }; }
    public static Banner Banner2() { return new Banner { Title = "New Year Collection", SubTitle = "Get All New Items", BannerType = BannerType.BannerOne.ToString(), ColorHexCode = "#000000", BackgroundColorHexCode = "#F5F3EF", IsActive = true }; }

    public static List<Banner> GetDefaultHeaderSlider()
    {
        var defaultBanner = new List<Banner>();
        defaultBanner.Add(Banner1());
        defaultBanner.Add(Banner2());
        return defaultBanner;
    }
}