using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.Collections.Map;

namespace Commons.Json
{
    internal static class JsonUtils
    {
        public static string FormatJsonObject(LinkedHashedMap<string, JsonValue> valueMap)
        {
            var builder = new StringBuilder();
            builder.Append(JsonTokens.LeftBrace).AppendLine();
            var count = 0;
            var total = valueMap.Count;
            foreach (var item in valueMap)
            {
                builder.Append(JsonTokens.Tab).Append(JsonTokens.Quoter).Append((string)item.Key).Append(JsonTokens.Quoter).Append(JsonTokens.Colon).Append(JsonTokens.Space);
                using (var reader = new StringReader(item.Value.ToString()))
                {
                    builder.Append(reader.ReadLine());
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (null == line)
                        {
                            break;
                        }
                        builder.AppendLine().Append(JsonTokens.Tab).Append(line);
                    }
                }
                count++;
                if (count < total)
                {
                    builder.Append(JsonTokens.Comma).AppendLine();
                }
            }
            builder.AppendLine().Append(JsonTokens.RightBrace);
            return builder.ToString();
        }

        public static string FormatJsonValue(this object jsonValue)
        {
            var type = jsonValue.GetType();
            var str = string.Empty;
            if (type.IsPrimitive || type == typeof(bool))
            {
                str = jsonValue.ToString();
            }
            else if (type == typeof(string))
            {
                var builder = new StringBuilder();
                builder.Append(JsonTokens.Quoter).Append(jsonValue).Append(JsonTokens.Quoter);
                str = builder.ToString();
            }
            else if (type == typeof(JsonObject))
            {
                str = jsonValue.ToString();
            }
            else if (type.IsArray)
            {
                var items = jsonValue as object[];
                var builder = new StringBuilder();
                var count = 0;
                var total = items.Length;
                builder.Append(JsonTokens.LeftBracket).AppendLine().Append(JsonTokens.Tab);
                foreach (var item in items)
                {
                    builder.Append(item.ToString());
                    count++;
                    if (count < total)
                    {
                        builder.Append(JsonTokens.Comma).Append(JsonTokens.Space);
                    }
                }
                builder.AppendLine().Append(JsonTokens.RightBracket);
                str = builder.ToString();
            }
            return str;
        }
    }
}
