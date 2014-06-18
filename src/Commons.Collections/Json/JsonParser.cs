// Copyright CommonsForNET 2014.
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
using System.IO;
using System.Text;

using Commons.Collections.Common;
using System.Globalization;

namespace Commons.Collections.Json
{
    internal static class JsonParser
    {
        private const string InvalidFormat = "The input JSON string format is invalid";
        private const char LeftBrace = '{';
        private const char RightBrace = '}';
        private const char LeftBracket = '[';
        private const char RightBracket = ']';
        private const char Comma = ',';
        private const char Colon = ':';
        private const char Quoter = '"';
        private const char Dot = '.';
        public static JsonObject ToJson(this string json)
        {
            Guarder.ValidateString(json);
            return ParseJson(json);
        }

        private static JsonObject ParseJson(string json)
        {
            JsonObject jsonObj = null;
            using (var reader = new StringReader(json.Trim()))
            {
                var charStack = new Stack<char>();
                var objectStack = new Stack<object>();
                var currentFragment = new StringBuilder();
                int intCh;
                while ((intCh = reader.Read())!= -1)
                {
                    var ch = Convert.ToChar(intCh);
                    if (charStack.Count == 0 && ch != LeftBrace)
                    {
                        throw new ArgumentException(InvalidFormat);
                    }
                    switch (ch)
                    {
                        case LeftBrace:
                            charStack.Push(ch);
                            objectStack.Push(new JsonObject());
                            break;
                        case LeftBracket:
                            break;
                        case RightBracket:
                            ExtractJsonValue(currentFragment, charStack, objectStack);
                            charStack.Push(ch);
                            break;
                        case RightBrace:
                            ExtractJsonValue(currentFragment, charStack, objectStack);
                            charStack.Pop().Verify(x => x == LeftBrace);
                            var inner = objectStack.Pop() as JsonObject;
                            if (objectStack.Count > 0)
                            {
                                var str = objectStack.Pop() as string;
                                str.Verify(x => !string.IsNullOrEmpty(x));
                                dynamic outer = objectStack.Peek();
                                outer[str] = inner;
                            }
                            else
                            {
                                jsonObj = inner;
                            }
                            charStack.Push(ch);
                            break;
                        case Comma:
                            if (charStack.Peek() == RightBrace || charStack.Peek() == RightBracket)
                            {
                                currentFragment.ToString().Trim().Verify(x => string.IsNullOrEmpty(x));
                                charStack.Pop();
                            }
                            else
                            {
                                ExtractJsonValue(currentFragment, charStack, objectStack);
                            }
                            break;
                        case Colon:
                            charStack.Pop().Verify(x => x == Quoter);
                            var keyStr = currentFragment.ToString().Trim();
                            keyStr.Verify(x => x[0] == Quoter && x[x.Length - 1] == Quoter);
                            keyStr = keyStr.Substring(1, keyStr.Length - 2);
                            objectStack.Push(keyStr);
                            currentFragment.Clear();
                            break;
                        case Quoter:
                            if (charStack.Peek() != Quoter)
                            {
                                charStack.Push(ch);
                            }
                            currentFragment.Append(ch);
                            break;
                        default:
                            currentFragment.Append(ch);
                            break;
                    }
                }

                return jsonObj;
            }
        }

        private static void ExtractJsonValue(StringBuilder currentFragment, Stack<char> charStack, Stack<object> objectStack)
        {
            object jsonValue;
            var value = currentFragment.ToString().Trim();
            var boolValue = false;
            if (charStack.Peek() == Quoter)
            {
                charStack.Pop().Verify(x => x == Quoter);
                value.Verify(x => !string.IsNullOrEmpty(x));
                value.Verify(x => x[0] == Quoter && x[x.Length - 1] == Quoter);
                jsonValue = value.Substring(1, value.Length - 2);
            }
            else if (bool.TryParse(value, out boolValue))
            {
                jsonValue = boolValue;
            }
            else if (value.ToLower(CultureInfo.InvariantCulture) == "null")
            {
                jsonValue = null;
            }
            else
            {
                if (value.IndexOf(Dot) != -1)
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
            var key = objectStack.Pop() as string;
            key.Verify(x => !string.IsNullOrEmpty(x));
            dynamic jsonObject = objectStack.Peek();
            jsonObject[key] = jsonValue;
            currentFragment.Clear();
        }

        private static void Verify<T>(this T x, Predicate<T> check)
        {
            if (!check(x))
            {
                throw new ArgumentException(InvalidFormat);
            }
        }
    }
}
