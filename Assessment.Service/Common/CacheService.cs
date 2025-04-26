using Assessment.Contract.Interfaces.Common;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Assessment.Service.Common
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private ConcurrentDictionary<string, DateTime> _keys = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// Get or add a value to the cache. If the value is not found in the cache, it will be added using the provided function.
        /// </summary>
        /// <param name="key">The key of cache</param>
        /// <param name="function">This function called when cache is empty to generate value for cache</param>
        /// <param name="options">Expiration of cache</param>
        public async Task<T> GetOrAdd<T>(string key, Func<Task<T>> function, DistributedCacheEntryOptions options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key)) return default;
            _keys.AddOrUpdate(key, s => DateTime.Now, (s, time) => DateTime.Now);
            var result = await cache.GetAsync(key, cancellationToken);
            if (result == null)
            {
                var semaphoreLock = _locks.GetOrAdd(key, s => new SemaphoreSlim(1, 1));
                // Wait for the lock to be released to prevent concurrent requests from generating the same value
                await semaphoreLock.WaitAsync(cancellationToken);
                try
                {
                    //Check if the value is still not in the cache after acquiring the lock
                    result = await cache.GetAsync(key, cancellationToken);
                    if (result == null)
                    {
                        // Call the function to generate the value
                        var data = await function.Invoke();
                        if (data != null && !data.Equals(default))
                        {
                            // Add generated value to cache
                            await SetAsync(key, data, options, cancellationToken);
                        }

                        return data;
                    }
                }
                finally
                {
                    semaphoreLock.Release();
                }
            }

            return JsonSerializer.Deserialize<T>(result);
        }

        private async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key)) return;
            _keys.AddOrUpdate(key, s => DateTime.Now, (s, time) => DateTime.Now);

            var data = JsonSerializer.SerializeToUtf8Bytes(value);

            if (options != null)
                await cache.SetAsync(key, data, options, cancellationToken);
            else
                await cache.SetAsync(key, data, cancellationToken);
        }
    }
}
