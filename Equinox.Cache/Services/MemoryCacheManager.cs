using System.Collections.Concurrent;
using Equinox.Cache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Equinox.Cache.Services
{
    /// <summary>
    /// Represents a memory cache manager
    /// </summary>
    public class MemoryCacheManager : CacheKeyService, ILocker, IStaticCacheManager
    {
        #region Fields

        // Flag: Has Dispose already been called?
        private bool _disposed;

        private readonly IMemoryCache _memoryCache;

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new();

        private static CancellationTokenSource _clearToken = new CancellationTokenSource();

        #endregion

        #region Ctor

        public MemoryCacheManager(IMemoryCache memoryCache) : base()
        {
            _memoryCache = memoryCache;
        }

        #endregion

        #region Utilities
        private MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            //set expiration time for the passed cache key
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(key.CacheTime)
            };
            
            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
            foreach (var keyPrefix in key.Prefixes.ToList())
            {
                var tokenSource = _prefixes.GetOrAdd(keyPrefix, new CancellationTokenSource());
                options.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));
            }

            return options;
        }
        private void Remove(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            cacheKey = PrepareKey(cacheKey, cacheKeyParameters);
            _memoryCache.Remove(cacheKey.Key);
        }
        private void Set(CacheKey key, object data)
        {
            if ((key?.CacheTime ?? 0) <= 0 || data == null)
                return;

            _memoryCache.Set(key.Key, data, PrepareEntryOptions(key));
        }

        #endregion

        #region Methods
        public Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            Remove(cacheKey, cacheKeyParameters);

            return Task.CompletedTask;
        }
        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            if ((key?.CacheTime ?? 0) <= 0)
                return await acquire();

            if (_memoryCache.TryGetValue(key.Key, out T result))
                return result;

            result = await acquire();

            if (result != null)
                await SetAsync(key, result);

            return result;
        }
        public async Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
        {
            if ((key?.CacheTime ?? 0) <= 0)
                return acquire();

            var result = _memoryCache.GetOrCreate(key.Key, entry =>
            {
                entry.SetOptions(PrepareEntryOptions(key));

                return acquire();
            });

            //do not cache null value
            if (result == null)
                await RemoveAsync(key);

            return result;
        }

        public T Get<T>(CacheKey key, Func<T> acquire)
        {
            if ((key?.CacheTime ?? 0) <= 0)
                return acquire();

            if (_memoryCache.TryGetValue(key.Key, out T result))
                return result;

            result = acquire();

            if (result != null)
                Set(key, result);

            return result;
        }

        /// <summary>
        /// Add the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task SetAsync(CacheKey key, object data)
        {
            Set(key, data);

            return Task.CompletedTask;
        }
        public Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            prefix = PrepareKeyPrefix(prefix, prefixParameters);

            _prefixes.TryRemove(prefix, out var tokenSource);
            tokenSource?.Cancel();
            tokenSource?.Dispose();

            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();

            _clearToken = new CancellationTokenSource();

            foreach (var prefix in _prefixes.Keys.ToList())
            {
                _prefixes.TryRemove(prefix, out var tokenSource);
                tokenSource?.Dispose();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _memoryCache.Dispose();

            _disposed = true;
        }
        
        public void PerformActionWithLock(string key, TimeSpan? expirationTime, Action action,
            bool throwIfLocked = false)
        {
            //ensure that lock is acquired
            if (IsActionLocked(key))
                if (throwIfLocked) throw new Exception($"Operation with key : '{key}' is locked");
                else return;

            try
            {
                if (expirationTime != null)
                {
                    _memoryCache.Set(key, key, expirationTime.Value);
                }
                else
                {
                    _memoryCache.Set(key, key);
                }

                //perform action
                action();
            }
            catch (Exception)
            {
                _memoryCache.Remove(key);
                throw;
            }
            finally
            {
                //release lock even if action fails
                _memoryCache.Remove(key);
            }
        }

        /// <inheritdoc cref="ILocker.PerformActionWithLock"/>
        public async Task PerformAsyncActionWithLock(string key, TimeSpan? expirationTime, Func<Task> action,
            bool throwIfLocked = false)
        {
            //ensure that lock is acquired
            if (IsActionLocked(key))
                if (throwIfLocked)
                    throw new Exception($"Operation with key : '{key}' is locked");
                else return;

            try
            {
                await LockAsync(key, expirationTime);
                //perform action
                await action();
            }
            catch (Exception)
            {
                _memoryCache.Remove(key);
                throw;
            }
            finally
            {
                //release lock even if action fails
                _memoryCache.Remove(key);
            }
        }

        public void Lock(string key, TimeSpan? expirationTime)
        {
            if (expirationTime != null)
            {
                _memoryCache.Set(key, key, expirationTime.Value);
            }
            else
            {
                _memoryCache.Set(key, key);
            }
        }

        public Task LockAsync(string key, TimeSpan? expirationTime)
        {
            Lock(key, expirationTime);
            return Task.CompletedTask;
        }

        public Task UnlockAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        public void Unlock(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <inheritdoc cref="ILocker.PerformActionWithLock"/>
        public bool IsActionLocked(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        #endregion
    }
}