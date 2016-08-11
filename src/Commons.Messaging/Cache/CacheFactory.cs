using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging.Cache
{
    public class CacheFactory : ICacheFactory
    {
        public ICache<K, T> Create<K, T>(string name)
        {
            return new SimpleCache<K, T>(name);
        }
    }
}
