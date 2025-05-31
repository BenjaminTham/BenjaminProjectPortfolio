namespace Ecommerce.Web.Mvc.Helpers;

public class JsonResponse
{
    public bool Success { get; set; }
    public string Item { get; set; }
    public string Message { get; set; }
    public dynamic Data { get; set; }
    public dynamic Error { get; set; }
}
