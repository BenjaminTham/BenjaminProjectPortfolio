using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Application.Services;

public class MemoryCacheManager : IMemoryCacheManager
{
    private readonly IMemoryCache _memoryCache;
    public MemoryCacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public void AppConfigurationRestore()
    {
        _memoryCache.Remove(AppMemoryCache.AppConfiguration);
    }
}
