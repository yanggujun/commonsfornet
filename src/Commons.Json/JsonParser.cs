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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Commons.Collections.Map;
using Commons.Utils;

namespace Commons.Json
{
    internal static class JsonParser
    {
        private static readonly char[] SpecialChars = { JsonTokens.LeftBrace, JsonTokens.RightBrace, JsonTokens.LeftBracket, JsonTokens.RightBracket, JsonTokens.Comma, JsonTokens.Colon };

        public static JsonObject ParseJsonObject(string json)
        {
            JsonObject jsonObj = null;
            using (var reader = new StringReader(json.Trim()))
            {
                var charStack = new Stack<char>();
                var objectStack = new Stack<object>();
                var currentFragment = new StringBuilder();
                var currentIsQuoted = false;
                int intCh;
                while ((intCh = reader.Read())!= -1)
                {
                    var ch = Convert.ToChar(intCh);
                    if (charStack.Count == 0 && ch != JsonTokens.LeftBrace)
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                    if (currentIsQuoted && SpecialChars.Any(x => x == ch))
                    {
                        currentFragment.Append(ch);
                        continue;
                    }
                    switch (ch)
                    {
                        case JsonTokens.LeftBrace:
                            charStack.Push(ch);
                            objectStack.Push(new JsonObject());
                            break;
                        case JsonTokens.LeftBracket: //[
                            objectStack.Push(new ArrayList());
                            charStack.Push(ch);
                            break;
                        case JsonTokens.RightBracket: //]
                            if (charStack.Peek() != JsonTokens.LeftBracket || currentFragment.ToString().Trim() != string.Empty)
                            {
                                ExtractJsonValue(charStack, currentFragment, objectStack);
                            }
                            charStack.Pop().Verify(x => x == JsonTokens.LeftBracket);
                            var array = objectStack.Pop() as ArrayList;
                            array.Verify(x => x != null);
                            var key = objectStack.Pop() as string;
                            key.Verify(x => !string.IsNullOrWhiteSpace(x));
                            dynamic outerObj = objectStack.Peek();
                            outerObj[key] = array;
                            charStack.Push(ch);
                            break;
                        case JsonTokens.RightBrace:
                            //Potential problems here
                            if (charStack.Peek() != JsonTokens.LeftBrace || currentFragment.ToString().Trim() != string.Empty)
                            {
                                ExtractJsonValue(charStack, currentFragment, objectStack);
                            }
                            charStack.Pop().Verify(x => x == JsonTokens.LeftBrace);
                            var inner = objectStack.Pop() as JsonObject;
                            if (objectStack.Count > 0)
                            {
                                if (charStack.Count > 0 && charStack.Peek() == JsonTokens.LeftBracket)
                                {
                                    var lastArray = objectStack.Peek() as ArrayList;
                                    lastArray.Verify(x => x != null);
                                    lastArray.Add(inner);
                                }
                                else
                                {
                                    var str = objectStack.Pop() as string;
                                    str.Verify(x => !string.IsNullOrWhiteSpace(x));
                                    dynamic outer = objectStack.Peek();
                                    outer[str] = inner;
                                }
                                charStack.Push(ch);
                            }
                            else
                            {
                                jsonObj = inner;
                            }
                            break;
                        case JsonTokens.Comma:
                            ExtractJsonValue(charStack, currentFragment, objectStack);
                            break;
                        case JsonTokens.Colon:
                            charStack.Pop().Verify(x => x == JsonTokens.Quoter);
                            var keyStr = currentFragment.ToString().Trim();
                            keyStr.Verify(x => x[0] == JsonTokens.Quoter && x[x.Length - 1] == JsonTokens.Quoter);
                            keyStr = keyStr.Substring(1, keyStr.Length - 2);
                            objectStack.Push(keyStr);
                            currentFragment.Clear();
                            break;
                        case JsonTokens.Quoter:
                            if (charStack.Peek() != JsonTokens.Quoter)
                            {
                                charStack.Push(ch);
                                currentIsQuoted = true;
                            }
                            else
                            {
                                currentIsQuoted = false;
                            }
                            currentFragment.Append(ch);
                            break;
                        default:
                            currentFragment.Append(ch);
                            break;
                    }
                }
                if (charStack.Count > 0)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }

                return jsonObj;
            }
        }

        public static JsonArray ParseJsonArray(string json)
        {
            return null;
        }

        private static void ExtractJsonValue(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            if (charStack.Peek() == JsonTokens.RightBrace || charStack.Peek() == JsonTokens.RightBracket)
            {
                currentFragment.ToString().Trim().Verify(x => string.IsNullOrWhiteSpace(x));
                charStack.Pop();
            }
            else
            {
                ParseJsonValue(currentFragment, charStack, objectStack);
            }
        }

        private static void ParseJsonValue(StringBuilder currentFragment, Stack<char> charStack, Stack<object> objectStack)
        {
            object jsonValue;
            var value = currentFragment.ToString().Trim();
            var boolValue = false;
            if (charStack.Peek() == JsonTokens.Quoter)
            {
                charStack.Pop();
                value.Verify(x => !string.IsNullOrWhiteSpace(x));
                value.Verify(x => x[0] == JsonTokens.Quoter && x[x.Length - 1] == JsonTokens.Quoter);
                jsonValue = value.Trim().Trim(JsonTokens.Quoter);
            }
            else if (bool.TryParse(value, out boolValue))
            {
                jsonValue = boolValue;
            }
            else if (value.ToLower(CultureInfo.InvariantCulture) == JsonTokens.Null)
            {
                jsonValue = JsonTokens.Null;
            }
            else
            {
                if (value.IndexOf(JsonTokens.Dot) != -1)
                {
                    double number = 0;
                    value.Verify(x => double.TryParse(x, out number));
                    jsonValue = number;
                }
                else
                {
                    var number = 0;
                    value.Verify(x => int.TryParse(x, out number));
                    jsonValue = number;
                }
            }
            var last = objectStack.Peek();
            if (last is ArrayList)
            {
                var array = last as ArrayList;
                array.Add(jsonValue);
            }
            else
            {
                var key = objectStack.Pop() as string;
                key.Verify(x => !string.IsNullOrWhiteSpace(x));
                dynamic jsonObject = objectStack.Peek();
                jsonObject[key] = jsonValue;
            }
            currentFragment.Clear();
        }

        private static void Verify<T>(this T x, Predicate<T> check)
        {
            if (!check(x))
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }
    }
}
