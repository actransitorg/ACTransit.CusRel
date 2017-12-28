using System.Runtime.Caching;

namespace ACTransit.Framework.Caching
{
    public interface ICache
    {
        /// <summary>
        /// Add a short Cache to the instanse. ( 60 seconds). 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration">AbsoluteExpiration in Minutes, defualt is 60 seconds</param>
        void AddShortCache(string key, object value, int absoluteExpiration = 60);

        /// <summary>
        /// Add a short Cache to the instanse.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration">AbsoluteExpiration in Minutes, defualt is 20 minutes</param>
        void AddCache(string key, object value, int absoluteExpiration = 20);

        void AddCache(string key, object value, CacheItemPolicy policy);

        T GetCache<T>(string key) where T : class;

        object GetCache(string key);

        void Remove(string key);

        void ClearAll();

    }
}
