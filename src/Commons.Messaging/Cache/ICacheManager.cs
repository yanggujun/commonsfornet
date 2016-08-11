using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging.Cache
{
    public interface ICacheManager
    {
        ICache<K, T> NewCache<K, T>(string name);
        void RemoveCache(string name);
    }
}
