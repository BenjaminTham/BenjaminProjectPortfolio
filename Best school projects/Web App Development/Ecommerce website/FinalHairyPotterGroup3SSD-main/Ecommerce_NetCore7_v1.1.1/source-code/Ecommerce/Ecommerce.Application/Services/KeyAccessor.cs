using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Application.Services;

public class KeyAccessor : IKeyAccessor
{
    private readonly IDataContext _db;
    private readonly IMemoryCache _memoryCache;
    public KeyAccessor(IDataContext db, IMemoryCache memoryCache)
    {
        _db = db;
        _memoryCache = memoryCache;
    }
    public string this[string key] { get => this.GetSection(key); }
    public string GetSection(string key)
    {
        IList<AppConfiguration> appConfiguration;

        if (!_memoryCache.TryGetValue(AppMemoryCache.AppConfiguration, out appConfiguration))
        {
            appConfiguration = _db.AppConfigurations.ToList();
            _memoryCache.Set(AppMemoryCache.AppConfiguration, appConfiguration);
        }
        appConfiguration = _memoryCache.Get(AppMemoryCache.AppConfiguration) as IList<AppConfiguration>;

        var getById = appConfiguration?.Where(u => u.Key == key).Select(o => o.Value).FirstOrDefault();
        return getById;

    }
}
