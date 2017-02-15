using System.Collections.Generic;

namespace Commons.Json.Mapper
{
	internal class ConfigContainer
    {
        private readonly Dictionary<string, object> configure = new Dictionary<string, object>();
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
