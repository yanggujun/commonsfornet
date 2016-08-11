using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Messaging.Cache
{
    public interface ICacheFactory
    {
        ICache<K, T> Create<K, T>(string name);
    }
}
