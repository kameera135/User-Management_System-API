
using Microsoft.Extensions.Caching.Memory;

namespace SITCAMSClientIntegration.MemoryCaches
{
    /// <summary>
    /// A custom memory cache used in order to not interefer with any
    /// application memory caches
    /// </summary>
    public class HandlerMemoryCache
    {
        public MemoryCache Cache { get; private set; }
        public HandlerMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions { });
        }
    }
}