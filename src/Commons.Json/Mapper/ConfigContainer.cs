using Commons.Collections.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Json.Mapper
{
    internal class ConfigContainer
    {
        private readonly HashedMap<string, object> configure = new HashedMap<string, object>();
        public bool TryGetValue(string key, out object value)
        {
            return configure.TryGetValue(key, out value);
        }

        public void Add(string key, object value)
        {
            configure[key] = value;
        }
    }
}
