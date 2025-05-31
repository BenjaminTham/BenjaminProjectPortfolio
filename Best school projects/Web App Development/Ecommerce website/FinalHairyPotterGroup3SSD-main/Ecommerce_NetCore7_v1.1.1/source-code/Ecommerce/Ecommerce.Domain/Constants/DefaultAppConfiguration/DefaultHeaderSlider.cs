using Ecommerce.Domain.Models;

namespace Ecommerce.Domain.Constants;

public static class DefaultHeaderSlider
{
    public static HeaderSlider HeaderSlider1() { return new HeaderSlider { Image = null, HeaderTextLineOne = "Start Buying", HeaderTextLineTwo = "Item One", SubText = "This is sub text for details info", IsActive = true }; }
    public static HeaderSlider HeaderSlider2() { return new HeaderSlider { Image = null, HeaderTextLineOne = "Start Buying", HeaderTextLineTwo = "Item Two", SubText = "This is sub text for details info", IsActive = true }; }
    public static HeaderSlider HeaderSlider3() { return new HeaderSlider { Image = null, HeaderTextLineOne = "Start Buying", HeaderTextLineTwo = "Item Three", SubText = "This is sub text for details info", IsActive = true }; }

    public static List<HeaderSlider> GetDefaultHeaderSlider()
    {
        var defaultHeaderSlider = new List<HeaderSlider>();
        defaultHeaderSlider.Add(HeaderSlider1());
        defaultHeaderSlider.Add(HeaderSlider2());
        defaultHeaderSlider.Add(HeaderSlider3());
        return defaultHeaderSlider;
    }
}

