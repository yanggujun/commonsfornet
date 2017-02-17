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

using System;
using System.Globalization;
using System.Text;

namespace Commons.Json.Mapper
{
    public class JsonParseEngine : IParseEngine
    {
        public JValue Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            var text = json.Trim();
            var firstCh = text[0];
            JValue result;
            if (firstCh.Equals(JsonTokens.LeftBrace))
            {
                result = ParseObject(json);
            }
            else if (firstCh.Equals(JsonTokens.LeftBracket))
            {
                result = ParseArray(json);
            }
            else if (firstCh.Equals(JsonTokens.Quoter))
            {
                result = ParseString(json);
            }
            else if (firstCh.Equals('N') || firstCh.Equals('n'))
            {
                result = ParseNull(json);
            }
            else if (firstCh.Equals('T') || firstCh.Equals('t') 
                || firstCh.Equals('F') || firstCh.Equals('f'))
            {
                result = ParseBool(json);
            }
            else if (Char.IsNumber(firstCh) || firstCh.Equals('-'))
            {
                result = ParseNumber(json);
            }
            else
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }

            return result;
        }

        private JValue ParseString(string json)
        {
            var value = json.Trim();
            if (string.IsNullOrWhiteSpace(value) || value.Length < 2 || !value[0].Equals(JsonTokens.Quoter) || !value[value.Length - 1].Equals(JsonTokens.Quoter))
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            var str = value.Trim(JsonTokens.Quoter);
            return new JString(str);
        }

        private JValue ParseNull(string json)
        {
            var text = json.Trim();
            if (!text.Equals(JsonTokens.Null, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
			return new JNull();
        }

        private JValue ParseArray(string json)
        {
            var text = json.Trim();
            var fragment = new StringBuilder();
            var quoted = false;
            var bracketMatch = 0;
            var braceMatch = 0;
            var hasComma = false;
            var array = new JArray();

            for (var i = 0; i < text.Length; i++)
            {
                if (bracketMatch < 0)
                {
                    throw new ArgumentException();
                }
                var ch = text[i];
                if (ch.Equals(JsonTokens.Quoter))
                {
                    quoted = !quoted;
                }
                if (quoted)
                {
                    fragment.Append(ch);
                    continue;
                }

                if (hasComma && !ch.IsEmpty())
                {
                    if (ch != JsonTokens.RightBracket 
                        && ch != JsonTokens.RightBrace 
                        && ch != JsonTokens.Comma)
                    {
                        hasComma = false;
                    }
                    else
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                }

                if (ch.Equals(JsonTokens.LeftBracket))
                {
                    ++bracketMatch;
                    if (bracketMatch > 1)
                    {
                        fragment.Append(ch);
                    }
                }
                else if (ch.Equals(JsonTokens.LeftBrace))
                {
                    ++braceMatch;
                    fragment.Append(ch);
                }
                else if (ch.Equals(JsonTokens.RightBrace))
                {
                    --braceMatch;
                    fragment.Append(ch);
                }
                else if (ch.Equals(JsonTokens.Comma) && bracketMatch == 1 && braceMatch == 0)
                {
                    if (string.IsNullOrWhiteSpace(fragment.ToString().Trim()))
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                    AppendValue(array, fragment);
                    hasComma = true;
                }
                else if (ch.Equals(JsonTokens.RightBracket))
                {
                    --bracketMatch;
                    if (bracketMatch == 0)
                    {
                        AppendValue(array, fragment);
                        if (i < text.Length - 1)
                        {
                            throw new ArgumentException();
                        }
                    }
                    else if (bracketMatch > 0)
                    {
                        fragment.Append(ch);
                    }
                }
                else
                {
                    fragment.Append(ch);
                }
            }

            if (quoted || bracketMatch != 0)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }

            return array;
        }

        private JValue ParseNumber(string json)
        {
            var number = json.Trim();
            var dotIndex = number.IndexOf(JsonTokens.Dot);
            if (dotIndex == number.Length - 1 || dotIndex == 0)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
			return new JPrimitive(number);
        }

        private JValue ParseBool(string json)
        {
            var text = json.Trim();
            return new JPrimitive(text);
        }

        private JValue ParseObject(string text)
        {
            var json = text.Trim();
            var fragment = new StringBuilder(200);

            var quoted = false;
            var jsonObject = new JObject();
            var braceMatch = 0;
            var bracketMatch = 0;
            for (var pos = 0; pos < json.Length; pos++)
            {
                if (braceMatch < 0)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                var ch = json[pos];
                if (ch.Equals(JsonTokens.Quoter))
                {
                    quoted = !quoted;
                }

                if (quoted)
                {
                    fragment.Append(ch);
                    continue;
                }

                if (ch.Equals(JsonTokens.LeftBrace))
                {
                    ++braceMatch;
                    if (braceMatch > 1)
                    {
                        fragment.Append(ch);
                    }
                }
                else if (ch.Equals(JsonTokens.LeftBracket))
                {
                    ++bracketMatch;
                    fragment.Append(ch);
                }
                else if (ch.Equals(JsonTokens.RightBracket))
                {
                    --bracketMatch;
                    fragment.Append(ch);
                }
                else if (ch.Equals(JsonTokens.Colon) && braceMatch == 1 && bracketMatch ==0)
                {
                    var key = Parse(fragment.ToString());
                    jsonObject.PutKey(key as JString);
                    fragment.Clear();
                }
                else if (ch.Equals(JsonTokens.Comma) && braceMatch == 1 && bracketMatch == 0)
                {
                    var v = Parse(fragment.ToString());
                    jsonObject.PutObject(v);
                    fragment.Clear();
                }
                else if (ch.Equals(JsonTokens.RightBrace))
                {
                    --braceMatch;
                    if (braceMatch == 0)
                    {
                        var frag = fragment.ToString();
                        if (!string.IsNullOrWhiteSpace(frag))
                        {
                            var v = Parse(frag);
                            jsonObject.PutObject(v);
                            if (pos < json.Length - 1)
                            {
                                throw new ArgumentException(Messages.InvalidFormat);
                            }
                        }
                    }
                    else if (braceMatch > 0)
                    {
                        fragment.Append(ch);
                    }
                }
                else
                {
                    fragment.Append(ch);
                }
            }
            if (quoted || braceMatch != 0)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            if (!jsonObject.Validate())
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            return jsonObject;
        }

        private void AppendValue(JArray array, StringBuilder jsonFragment)
        {
            var text = jsonFragment.ToString();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var value = Parse(jsonFragment.ToString());
                array.Add(value);
            }
            jsonFragment.Clear();
        }
    }
}
