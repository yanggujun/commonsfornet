using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Json
{
    public class JsonArray : JsonValue
    {
        private List<JsonValue> values;
        public JsonArray(JsonValue[] values)
        {
            this.values = new List<JsonValue>(values);
        }

        public void Add(JsonValue value)
        {
            values.Add(value);
        }
    }
}
