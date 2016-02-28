using System;
using System.Dynamic;

namespace Commons.Json
{
    public class JsonPrimitive : JsonValue
    {
        private object primitive;
        public JsonPrimitive(object value)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (!type.IsPrimitive || type != typeof(string))
                {
                    throw new ArgumentException(Messages.InvalidValue);
                }
            }
        }
    }
}
