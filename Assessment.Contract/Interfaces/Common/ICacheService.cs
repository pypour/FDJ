using Microsoft.Extensions.Caching.Distributed;

namespace Assessment.Contract.Interfaces.Common
{
    public interface ICacheService
    {
        Task<T> GetOrAdd<T>(string key, Func<Task<T>> value, DistributedCacheEntryOptions options = null,
            CancellationToken cancellationToken = default);
    }
}
