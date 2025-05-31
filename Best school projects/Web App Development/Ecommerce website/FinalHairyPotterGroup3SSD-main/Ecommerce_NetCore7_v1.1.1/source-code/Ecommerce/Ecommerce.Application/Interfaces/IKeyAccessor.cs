namespace Ecommerce.Application.Interfaces;

public interface IKeyAccessor
{
    string this[string key] { get; }
    string GetSection(string key);
}
