using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Services;

public class CookieManager : ICookieService
{
    private readonly IHttpContextAccessor _contextAccessor;
    public CookieManager(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    public void Clear()
    {
        foreach (var cookie in _contextAccessor.HttpContext.Request.Cookies.Keys)
        {
            _contextAccessor.HttpContext.Response.Cookies.Delete(cookie);
        }
    }

    public bool Contains(string key)
    {
        return _contextAccessor.HttpContext.Request.Cookies.ContainsKey(key);
    }

    public string Get(string key)
    {
        var getCookie = _contextAccessor.HttpContext.Request.Cookies[key];
        return getCookie;
    }

    public void Remove(string key)
    {
        _contextAccessor.HttpContext.Response.Cookies.Delete(key);
    }

    public void Set(string key, string value, int? cacheTimeInMinute)
    {
        CookieOptions option = new CookieOptions();
        if (cacheTimeInMinute != null)
            option.Expires = DateTime.Now.AddMinutes((int)cacheTimeInMinute);
        else
            option.Expires = DateTime.Now.AddMinutes(60);
        _contextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
    }

}
