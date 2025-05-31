namespace Ecommerce.Application.Helpers;

public static class MinMaxVal
{
    public static string getMinMaxVal(decimal[] price)
    {
        if (price == null || price.Length == 0)
        {
            return "0";
        }
        decimal max = price.Max();
        decimal min = price.Min();
        string res = max == min ? max.ToString() : min.ToString() + " - " + max.ToString();
        return res;
    }
}
