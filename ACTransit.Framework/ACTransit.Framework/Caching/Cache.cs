using System;
using System.Runtime.Caching;

namespace ACTransit.Framework.Caching
{
    public class Cache:ICache
    {
        private readonly string _name;
        readonly object _lock = new object();
        ObjectCache _instance ;

        public Cache(string name)
        {
            _name = name;
            _instance = new MemoryCache(name);
        }
        
        private ObjectCache Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Add a short Cache to the instanse. ( Default is 60 seconds). 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>                
        public void AddShortCache(string key, object value, int absoluteExpiration = 60)
        {
            lock (_lock)
            {
                Instance.Add(key, value, DateTime.Now.AddSeconds(absoluteExpiration));                                
            }
        }

        /// <summary>
        /// Add a short Cache to the instanse.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration">AbsoluteExpiration in Minutes, defualt is 20 minutes</param>
        public void AddCache(string key, object value, int absoluteExpiration = 20)
        {
            lock (_lock)
            {   if (value!=null)
                    Instance.Add(key, value, DateTime.Now.AddMinutes(absoluteExpiration));
            }
        }

        public void AddCache(string key, object value, CacheItemPolicy policy)
        {
            lock (_lock)
            {
                if (policy==null)
                    policy=new CacheItemPolicy{AbsoluteExpiration = DateTime.Now.AddMinutes(20)};
                if (value!=null)
                    Instance.Add(key, value,policy );                
            }
        }

        public T GetCache<T>(string key) where T : class
        {
            return GetCache(key) as T;
        }

        public object GetCache(string key)
        {
            lock (_lock)
            {
                return _instance[key];
            }
        }

        public ObjectCache GetAllCache() 
        {
            lock (_lock)
            {
                return _instance;
            }
        }

        public void Remove(string key)
        {
            lock (_lock)
            {
                _instance.Remove(key);
            }
        }

        public void ClearAll()
        {
            lock (_lock)
            {                
                ((MemoryCache)_instance).Dispose();
                _instance = new MemoryCache(_name);
            }
        }
     
    }
}
