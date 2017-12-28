using System;
using System.Runtime.Caching;

namespace ACTransit.Framework.Caching
{
    public static class DomainCache
    {
        private const int TotalSeconds = 24 * 60;
        private static TimeSpan _timeToExpire = new TimeSpan(0, 9, 0, 0);
        private static readonly object Lock = new object();
        private static ObjectCache _instance = new MemoryCache("GlobalCache");

        private static ObjectCache Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance;
                }
            }
        }


        /// <summary>
        /// Add a very short cache to the instasnce. (5 seconds)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddVeryShortCache(string key, object value)
        {
            lock (Lock)
            {
                Instance.Add(key, value, DateTime.Now.AddSeconds(5));
                //Instance.Add(cacheKey, value, null, DateTime.Now.AddSeconds(5), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);                
            }
        }

        /// <summary>
        /// Add a short Cache to the instanse. ( 60 seconds). 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddShortCache(string key, object value)
        {
            lock (Lock)
            {
                Instance.Add(key, value, DateTime.Now.AddSeconds(60));
                //Instance.Add(cacheKey, value, null, DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);                
            }
        }

        /// <summary>
        /// Add a short Cache to the instanse.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration">AbsoluteExpiration in Minutes, defualt is 20 minutes</param>
        public static void AddCache(string key, object value, int absoluteExpiration = 20)
        {
            lock (Lock)
            {
                Instance.Add(key, value, DateTime.Now.AddMinutes(absoluteExpiration));
                //Instance.Add(cacheKey, value, null, DateTime.Now.AddMinutes(absoluteExpiration), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
        }

        public static T GetCache<T>(string key) where T : class
        {
            return GetCache(key) as T;
        }

        public static object GetCache(string key)
        {
            lock (Lock)
            {
                return _instance[key];
            }
        }

        public static void Remove(string key)
        {
            lock (Lock)
            {
                _instance.Remove(key);
            }
        }

        public static void ClearAll()
        {
            lock (Lock)
            {
                ((MemoryCache)_instance).Dispose();
                _instance = new MemoryCache("GlobalCache");
            }
        }

        public static double TillToNextDay
        {
            get
            {
                DateTime now = DateTime.Now;
                TimeSpan nowTime = now.TimeOfDay;
                var diff = nowTime.TotalMinutes - _timeToExpire.TotalMinutes;
                if (diff < 0)
                    return (int)Math.Abs(diff);
                else
                {
                    return (int)(TotalSeconds - diff);
                }

            }
        }

    }
}