using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Models;
using System.Text.Json;

namespace Ecommerce.Domain.Constants;

public static class DefaultAppConfiguration
{
    public static AppConfiguration HeaderSlider() { return new AppConfiguration { Key = AppConfigurationType.HeaderSlider, Value = JsonSerializer.Serialize(DefaultHeaderSlider.GetDefaultHeaderSlider()) }; }
    public static AppConfiguration GeneralConfiguration() { return new AppConfiguration { Key = AppConfigurationType.GeneralConfiguration, Value = JsonSerializer.Serialize(new GeneralConfiguration() { CurrencySymbol = "\u0024", CurrencyPosition = CurrencyPosition.Start, CompanyName = "EShop" }) }; }
    public static AppConfiguration FeatureProductConfiguration() { return new AppConfiguration { Key = AppConfigurationType.FeatureProductConfiguration, Value = JsonSerializer.Serialize(new List<FeatureProductConfiguration>()) }; }
    public static AppConfiguration TopCategoriesConfiguration() { return new AppConfiguration { Key = AppConfigurationType.TopCategoriesConfiguration, Value = JsonSerializer.Serialize(new List<TopCategoriesConfiguration>()) }; }
    public static AppConfiguration DealOfTheDayConfiguration() { return new AppConfiguration { Key = AppConfigurationType.DealOfTheDay, Value = JsonSerializer.Serialize(new DealOfTheDay()) }; }
    public static AppConfiguration SocialProfile() { return new AppConfiguration { Key = AppConfigurationType.SocialProfile, Value = JsonSerializer.Serialize(new SocialProfile()) }; }
    public static AppConfiguration StockConfiguration() { return new AppConfiguration { Key = AppConfigurationType.StockConfiguration, Value = JsonSerializer.Serialize(new StockConfiguration() { IsOutOfStockNotificationEnabled = true, IsLowStockNotificationEnabled = true, OutOfStockThreshold = 0, LowStockThreshold = 10, IsOutOfStockItemHidden = false }) }; }
    public static AppConfiguration BasicSeoConfiguration() { return new AppConfiguration { Key = AppConfigurationType.BasicSeoConfiguration, Value = JsonSerializer.Serialize(new BasicSeoConfiguration() { SeoTitle = "EShop - Shop Online for Branded Shoes, Clothing & Accessories", SeoDescription = "Online Shopping Site for Fashion & Lifestyle. Best Fashion Expert brings you a variety of footwear, Clothing, Accessories and lifestyle products for women & men. Best Online Fashion Store *COD *Easy returns and exchanges. Get all your desire product in one place.", SeoKeywords = "shop, product, ecommerce" }) }; }
    public static AppConfiguration SmtpConfiguration() { return new AppConfiguration { Key = AppConfigurationType.SmtpConfiguration, Value = JsonSerializer.Serialize(new SmtpConfiguration()) }; }
    public static AppConfiguration SecurityConfiguration() { return new AppConfiguration { Key = AppConfigurationType.SecurityConfiguration, Value = JsonSerializer.Serialize(new SecurityConfiguration()) }; }
    public static AppConfiguration AdvancedConfiguration() { return new AppConfiguration { Key = AppConfigurationType.AdvancedConfiguration, Value = JsonSerializer.Serialize(new AdvancedConfiguration() { IsComingSoonEnabled = false }) }; }
    public static AppConfiguration BannerConfiguration() { return new AppConfiguration { Key = AppConfigurationType.BannerConfiguration, Value = JsonSerializer.Serialize(DefaultBanner.GetDefaultHeaderSlider()) }; }


    public static List<AppConfiguration> GetDefaultAppConfiguration()
    {
        var defaultAppConfiguration = new List<AppConfiguration>();
        defaultAppConfiguration.Add(HeaderSlider());
        defaultAppConfiguration.Add(GeneralConfiguration());
        defaultAppConfiguration.Add(FeatureProductConfiguration());
        defaultAppConfiguration.Add(TopCategoriesConfiguration());
        defaultAppConfiguration.Add(DealOfTheDayConfiguration());
        defaultAppConfiguration.Add(SocialProfile());
        defaultAppConfiguration.Add(StockConfiguration());
        defaultAppConfiguration.Add(BasicSeoConfiguration());
        defaultAppConfiguration.Add(SmtpConfiguration());
        defaultAppConfiguration.Add(SecurityConfiguration());
        defaultAppConfiguration.Add(AdvancedConfiguration());
        defaultAppConfiguration.Add(BannerConfiguration());
        return defaultAppConfiguration;
    }
}

