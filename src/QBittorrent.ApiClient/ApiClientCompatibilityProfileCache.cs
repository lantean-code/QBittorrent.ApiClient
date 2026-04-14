using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace QBittorrent.ApiClient
{
    internal sealed class ApiClientCompatibilityProfileCache
    {
        private readonly ConcurrentDictionary<string, ApiClientCompatibilityProfile> _profiles;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;

        public ApiClientCompatibilityProfileCache()
        {
            _profiles = new ConcurrentDictionary<string, ApiClientCompatibilityProfile>(StringComparer.Ordinal);
            _locks = new ConcurrentDictionary<string, SemaphoreSlim>(StringComparer.Ordinal);
        }

        public bool TryGetValue(string cacheKey, [NotNullWhen(true)] out ApiClientCompatibilityProfile? profile)
        {
            ArgumentNullException.ThrowIfNull(cacheKey);

            return _profiles.TryGetValue(cacheKey, out profile);
        }

        public async Task<ApiResult<ApiClientCompatibilityProfile>> GetOrAddAsync(
            string cacheKey,
            Func<CancellationToken, Task<ApiResult<ApiClientCompatibilityProfile>>> valueFactory,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(cacheKey);
            ArgumentNullException.ThrowIfNull(valueFactory);

            if (TryGetValue(cacheKey, out var cachedProfile))
            {
                return ApiResult.CreateSuccess(cachedProfile);
            }

            var semaphore = GetOrAddLock(cacheKey);

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (TryGetValue(cacheKey, out cachedProfile))
                {
                    return ApiResult.CreateSuccess(cachedProfile);
                }

                var profileResult = await valueFactory(cancellationToken);
                if (profileResult.TryGetValue(out var profile))
                {
                    _profiles[cacheKey] = profile;
                }

                return profileResult;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<ApiResult> RefreshAsync(
            string cacheKey,
            Func<CancellationToken, Task<ApiResult<ApiClientCompatibilityProfile>>> valueFactory,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(cacheKey);
            ArgumentNullException.ThrowIfNull(valueFactory);

            var semaphore = GetOrAddLock(cacheKey);

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                _profiles.TryRemove(cacheKey, out _);

                var profileResult = await valueFactory(cancellationToken);
                if (profileResult.IsFailure)
                {
                    return profileResult.Failure.ToResult();
                }

                if (!profileResult.TryGetValue(out var profile))
                {
                    throw new InvalidOperationException("Expected a completed compatibility-profile result.");
                }

                _profiles[cacheKey] = profile;
                return ApiResult.CreateSuccess();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task TryHydrateAsync(string cacheKey, ApiClientCompatibilityProfile profile, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(cacheKey);
            ArgumentNullException.ThrowIfNull(profile);

            if (_profiles.ContainsKey(cacheKey))
            {
                return;
            }

            var semaphore = GetOrAddLock(cacheKey);

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                _profiles.TryAdd(cacheKey, profile);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private SemaphoreSlim GetOrAddLock(string cacheKey)
        {
            return _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
        }
    }
}
