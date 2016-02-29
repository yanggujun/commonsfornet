// Copyright CommonsForNET.
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using System.Text;
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
