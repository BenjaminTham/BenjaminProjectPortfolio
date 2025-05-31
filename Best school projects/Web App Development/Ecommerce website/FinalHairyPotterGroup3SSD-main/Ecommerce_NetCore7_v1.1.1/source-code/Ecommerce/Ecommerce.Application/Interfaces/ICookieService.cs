namespace Ecommerce.Application.Interfaces;

public interface ICookieService
{
    string Get(string key);
    void Set(string key, string data, int? cacheTimeInMinute);
    bool Contains(string key);
    void Remove(string key);
    void Clear();
}
