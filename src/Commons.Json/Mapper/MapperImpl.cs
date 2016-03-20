using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.Collections.Map;

namespace Commons.Json.Mapper
{
    internal class MapperImpl
    {
        private readonly HashedBimap<string, string> keyPropMap = new HashedBimap<string, string>();
        private Type type;
        public MapperImpl(Type type)
        {
            this.type = type;
        }

        public void Map(string key, string prop)
        {
            keyPropMap.Enforce(key, prop);
        }

        public string GetKey(string prop)
        {
            return keyPropMap.KeyOf(prop);
        }

        public string GetProp(string key)
        {
            return keyPropMap.ValueOf(key);
        }

        public bool TryGetKey(string prop, out string key)
        {
            return keyPropMap.TryGetKey(prop, out key);
        }

        public bool TryGetProperty(string key, out string property)
        {
            return keyPropMap.TryGetValue(key, out property);
        }

        public Func<object> Create {get;set;}

        public Func<List<object>, IEnumerable<object>> ArrayConverter { get; set; }
    }
}
