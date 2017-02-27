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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Commons.Json
{
    internal static class JsonParser
    {
        private static readonly char[] SpecialChars = { JsonTokens.LeftBrace, JsonTokens.RightBrace, JsonTokens.LeftBracket, JsonTokens.RightBracket, JsonTokens.Comma, JsonTokens.Colon };

        public static dynamic Parse(string json)
        {
            var charStack = new Stack<char>();
            var objectStack = new Stack<object>();
            var currentFragment = new StringBuilder();
            var currentIsQuoted = false;
            var hasComma = false;
            var text = json.Trim();
            foreach (var ch in text)
            {
                if (currentIsQuoted && SpecialChars.Any(x => x == ch))
                {
                    currentFragment.Append(ch);
                    continue;
                }

                if (hasComma && !ch.IsEmpty())
                {
                    if (ch != JsonTokens.RightBracket && ch != JsonTokens.RightBrace && ch != JsonTokens.Comma)
                    {
                        hasComma = false;
                    }
                    else
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                }

                switch (ch)
                {
                    case JsonTokens.LeftBrace:
                        OnLeftBrace(charStack, currentFragment, objectStack);
                        break;
                    case JsonTokens.LeftBracket: //[
                        OnLeftBracket(charStack, currentFragment, objectStack);
                        break;
                    case JsonTokens.RightBracket: //]
                        OnRightBracket(charStack, currentFragment, objectStack);
                        break;
                    case JsonTokens.RightBrace:
                        OnRightBrace(charStack, currentFragment, objectStack);
                        break;
                    case JsonTokens.Comma:
                        OnComma(charStack, currentFragment, objectStack);
                        hasComma = true;
                        break;
                    case JsonTokens.Colon:
                        OnColon(charStack, currentFragment, objectStack);
                        break;
                    case JsonTokens.Quoter:
                        OnQuoter(charStack, currentFragment, objectStack, ref currentIsQuoted);
                        break;
                    default:
                        currentFragment.Append(ch);
                        break;
                }
                if (charStack.Count > 0 && charStack.Peek() == JsonTokens.LeftBracket 
                                        && ch != JsonTokens.LeftBracket && !ch.IsEmpty())
                {
                    charStack.Push('a');
                }
            }

            if (charStack.Count > 0 || currentIsQuoted)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            JsonValue jsonValue;
            if (objectStack.Count == 0)
            {
                jsonValue = ParseJsonValue(currentFragment);
            }
            else 
            {
                jsonValue = objectStack.Pop() as JsonValue;
            }
            return jsonValue;
        }

        private static void OnLeftBrace(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            charStack.Push(JsonTokens.LeftBrace);
            objectStack.Push(new JsonObject());
        }

        private static void OnRightBrace(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            JsonValue value;
            charStack.Count.Verify(x => x > 0);
            var ch = charStack.Pop();
            ch.Verify(x => x == JsonTokens.LeftBrace || x == JsonTokens.Colon);
            if (ch == JsonTokens.Colon)
            {
                charStack.Pop().Verify(x => x == JsonTokens.LeftBrace);
                var text = currentFragment.ToString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    value = objectStack.Pop() as JsonValue;
                    value.Verify(x => x != null);
                }
                else
                {
                    value = ParseJsonValue(currentFragment);
                }
                var key = objectStack.Pop() as string;
                key.Verify(x => !string.IsNullOrWhiteSpace(x));
                var outer = objectStack.Peek() as JsonObject;
                outer.Verify(x => x != null);
                outer[key] = value;
            }
            else
            {
                currentFragment.ToString().Trim().Verify(x => string.IsNullOrWhiteSpace(x));
            }
        }

        private static void OnLeftBracket(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            charStack.Push(JsonTokens.LeftBracket);
            objectStack.Push(new JsonArray());
        }

        private static void OnRightBracket(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            JsonValue value;
            var ch = charStack.Pop();
            ch.Verify(x => x == 'a' || x == JsonTokens.LeftBracket);

            if (ch == 'a')
            {
                charStack.Pop().Verify(x => x == JsonTokens.LeftBracket);
                var text = currentFragment.ToString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    value = objectStack.Pop() as JsonValue;
                    value.Verify(x => x != null);
                }
                else
                {
                    value = ParseJsonValue(currentFragment);
                }
                objectStack.Count.Verify(x => x > 0);
                var outer = objectStack.Peek() as JsonArray;
                outer.Verify(x => x != null);
                outer.Add(value);
            }
        }

        private static void OnComma(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            JsonValue value;
            var ch = charStack.Peek();
            ch.Verify(x => x == JsonTokens.Colon || x == JsonTokens.LeftBracket || x == 'a');
            if (ch == JsonTokens.Colon)
            {
                charStack.Pop();
                charStack.Peek().Verify(x => x == JsonTokens.LeftBrace);
            }

            var text = currentFragment.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                value = objectStack.Pop() as JsonValue;
                value.Verify(x => x != null);
            }
            else
            {
                value = ParseJsonValue(currentFragment);
            }

            if (ch == JsonTokens.Colon)
            {
                var key = objectStack.Pop() as string;
                key.Verify(x => !string.IsNullOrWhiteSpace(x));
                var outer = objectStack.Peek() as JsonObject;
                outer.Verify(x => x != null);
                outer[key] = value;
            }
            else if (ch == 'a')
            {
                var array = objectStack.Peek() as JsonArray;
                array.Verify(x => x != null);
                array.Add(value);
            }
        }

        private static void OnColon(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            var key = currentFragment.ToString().Trim();
            charStack.Peek().Verify(x => x == JsonTokens.LeftBrace);
            objectStack.Peek().Verify(x => x is JsonObject);
            key.Verify(x => !string.IsNullOrWhiteSpace(x));
            key.Verify(x => x[0] == JsonTokens.Quoter && x[x.Length - 1] == JsonTokens.Quoter);
            key = key.Trim(JsonTokens.Quoter);
            objectStack.Push(key);
            charStack.Push(JsonTokens.Colon);
            currentFragment.Clear();
        }

        private static void OnQuoter(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack, ref bool quoted)
        {
            if (!quoted)
            {
                charStack.Push(JsonTokens.Quoter);
                quoted = true;
            }
            else
            {
                charStack.Pop().Verify(x => x == JsonTokens.Quoter);
                quoted = false;
            }
            currentFragment.Append(JsonTokens.Quoter);
        }

        private static JsonValue ParseJsonValue(StringBuilder currentFragment)
        {
            JsonValue jsonValue;
            var value = currentFragment.ToString().Trim();
            var boolValue = false;
            if (value[0] == JsonTokens.Quoter && value[value.Length - 1] == JsonTokens.Quoter)
            {
                value.Verify(x => !string.IsNullOrWhiteSpace(x));
                jsonValue = new JsonPrimitive((value.Trim().Trim(JsonTokens.Quoter)));
            }
            else if (bool.TryParse(value, out boolValue))
            {
                jsonValue = new JsonPrimitive(boolValue);
            }
            else if (value.ToLower() == JsonTokens.Null)
            {
                jsonValue = null;
            }
            else
            {
                var dotIndex = 0;
                if ((dotIndex = value.IndexOf(JsonTokens.Dot)) != -1)
                {
                    if (dotIndex == 0 || dotIndex == value.Length - 1)
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                    double number = 0;
                    value.Verify(x => double.TryParse(x, out number));
                    jsonValue = new JsonPrimitive(number);
                }
                else
                {
                    var integer = 0;
                    long longInteger = 0;
                    if (int.TryParse(value, out integer))
                    {
                        jsonValue = new JsonPrimitive(integer);
                    }
                    else if (long.TryParse(value, out longInteger))
                    {
                        jsonValue = new JsonPrimitive(longInteger);
                    }
                    else
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                }
            }
            currentFragment.Clear();

            return jsonValue;
        }
    }
}
