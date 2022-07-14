namespace Equinox.Cache.Services
{
    public interface ILocker
    {
        
        
        /// <summary>
        /// Perform some action with exclusive in-memory lock
        /// </summary>
        /// <param name="key">The key we are locking on</param>
        /// <param name="expirationTime"></param>
        /// <param name="action">Action to be performed with locking</param>
        /// <param name="throwIfLocked"></param>
        void PerformActionWithLock(string key, TimeSpan? expirationTime, Action action, bool throwIfLocked = false);

        /// <summary>
        /// Perform some async action with exclusive lock
        /// </summary>
        /// <param name="key">The key we are locking on</param>
        /// <param name="expirationTime">The time after which the lock will automatically be expired</param>
        /// <param name="action">Action to be performed with locking</param>
        /// <param name="throwIfLocked"></param>
        Task PerformAsyncActionWithLock(string key, TimeSpan? expirationTime, Func<Task> action,
            bool throwIfLocked = false);

        void Lock(string key, TimeSpan? expirationTime);

        Task LockAsync(string key, TimeSpan? expirationTime);

        Task UnlockAsync(string key);

        void Unlock(string key);
        
        /// <inheritdoc cref="MemoryCacheManager.IsActionLocked"/>
        bool IsActionLocked(string resource);
    }
}