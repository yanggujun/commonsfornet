using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.Collections.Map;

namespace Commons.Messaging.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly HashedMap<string, ICache> caches = new HashedMap<string, ICache>();
        private readonly ICacheFactory cacheFactory;

        public CacheManager(ICacheFactory cacheFactory)
        {
            this.cacheFactory = cacheFactory;
        }

        public ICache<K, T> NewCache<K, T>(string name)
        {
            if (caches.ContainsKey(name))
            {
                throw new InvalidOperationException(string.Format("The cache with name {0} already exists.", name));
            }
            var cache = cacheFactory.Create<K, T>(name);
            caches.Add(name, cache);

            return cache;
        }

        public void RemoveCache(string name)
        {
            if (caches.ContainsKey(name))
            {
                caches.Remove(name);
            }
        }
    }
}
