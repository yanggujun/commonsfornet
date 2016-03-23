using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Commons.Collections.Map;
using Commons.Collections.Set;

namespace Commons.Json.Mapper
{
    internal class MapperImpl
    {
        private readonly HashedBimap<string, string> keyPropMap = new HashedBimap<string, string>();
		private readonly HashedSet<string> ignoredProps = new HashedSet<string>();

        public void Map(string key, string prop)
        {
            keyPropMap.Enforce(key, prop);
        }

	    public void IgnoreProperty(string prop)
	    {
		    ignoredProps.Add(prop);
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

	    public IReadOnlyStrictSet<string> IgnoredProperties
	    {
		    get { return ignoredProps; }
	    }

        public Func<object> Create {get;set;}

        public Func<List<object>, IEnumerable<object>> ArrayConverter { get; set; }

		public string DateFormat { get; set; }
    }
}
