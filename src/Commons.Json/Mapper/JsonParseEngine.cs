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
using System.Text;

using Commons.Utils;

namespace Commons.Json.Mapper
{
	internal sealed class JsonParseEngine
    {
        public JValue Parse(string json)
        {
            var charStack = new Stack<char>();
            var objectStack = new Stack<object>();
            var currentFragment = new StringBuilder();
            var currentIsQuoted = false;
            var hasComma = false;
			var text = json.Trim();
			for (var i = 0; i < text.Length; i++)
            {
				var ch = text[i];

				if (currentIsQuoted && IsSpecial(ch))
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
					charStack.Push(JsonTokens.NonEmptyArrayMark);
				}
            }

            if (charStack.Count > 0 || currentIsQuoted)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            JValue jsonValue;
            if (objectStack.Count == 0)
            {
                jsonValue = ParseJsonValue(currentFragment.ToString().Trim());
            }
            else 
            {
                jsonValue = objectStack.Pop() as JValue;
            }
            return jsonValue;
        }

        private static void OnLeftBrace(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            charStack.Push(JsonTokens.LeftBrace);
            objectStack.Push(new JObject());
        }

        private static void OnRightBrace(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            JValue value;
            var ch = charStack.Pop();

			if (ch != JsonTokens.LeftBrace && ch != JsonTokens.Colon)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}

			if (ch == JsonTokens.Colon)
			{
				if (charStack.Pop() != JsonTokens.LeftBrace)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
				var text = currentFragment.ToString().Trim();
				if (string.IsNullOrEmpty(text))
				{
					value = objectStack.Pop() as JValue;
					if (value == null)
					{
						throw new ArgumentException(Messages.InvalidFormat);
					}
				}
				else
				{
					value = ParseJsonValue(text);
					currentFragment.Length = 0;
				}

				var key = objectStack.Pop() as string;
				if (string.IsNullOrWhiteSpace(key))
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
				var outer = objectStack.Peek() as JObject;
				if (outer == null)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
				outer[key] = value;
			}
			else
			{
				if (!string.IsNullOrEmpty(currentFragment.ToString().Trim()))
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
			}
        }

        private static void OnLeftBracket(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            charStack.Push(JsonTokens.LeftBracket);
            objectStack.Push(new JArray());
        }

        private static void OnRightBracket(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
			if (charStack.Count == 0)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}
            JValue value;

            var ch = charStack.Pop();
			if (ch != JsonTokens.NonEmptyArrayMark && ch != JsonTokens.LeftBracket)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}

            if (ch == JsonTokens.NonEmptyArrayMark)
            {
				if (charStack.Pop() != JsonTokens.LeftBracket)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
                var text = currentFragment.ToString().Trim();
                if (string.IsNullOrEmpty(text))
                {
                    value = objectStack.Pop() as JValue;
					if (value == null)
					{
						throw new ArgumentException(Messages.InvalidFormat);
					}
                }
                else
                {
                    value = ParseJsonValue(text);
					currentFragment.Clear();
                }
				if (objectStack.Count == 0)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
                var outer = objectStack.Peek() as JArray;
				if (outer == null)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
                outer.Add(value);
            }
        }

        private static void OnComma(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            JValue value;
			if (charStack.Count == 0)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}
            var ch = charStack.Peek();
			if (ch != JsonTokens.Colon && ch != JsonTokens.LeftBracket && ch != JsonTokens.NonEmptyArrayMark)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}
            if (ch == JsonTokens.Colon)
            {
                charStack.Pop();
				if (charStack.Peek() != JsonTokens.LeftBrace)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
            }

			var text = currentFragment.ToString().Trim();
            if (string.IsNullOrEmpty(text))
            {
                value = objectStack.Pop() as JValue;
				if (value == null)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
            }
            else
            {
                value = ParseJsonValue(text);
				currentFragment.Clear();
            }

            if (ch == JsonTokens.Colon)
            {
                var key = objectStack.Pop() as string;

				if (string.IsNullOrWhiteSpace(key))
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}

                var outer = objectStack.Peek() as JObject;
				if (outer == null)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
                outer[key] = value;
            }
            else if (ch == JsonTokens.NonEmptyArrayMark)
            {
                var array = objectStack.Peek() as JArray;
				if (array == null)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
                array.Add(value);
            }
        }

        private static void OnColon(Stack<char> charStack, StringBuilder currentFragment, Stack<object> objectStack)
        {
            var key = currentFragment.ToString().Trim();
            //objectStack.Peek().Verify(x => x is JObject);

			if (charStack.Peek() != JsonTokens.LeftBrace)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}

			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}
			if (key[0] != JsonTokens.Quoter || key[key.Length - 1] != JsonTokens.Quoter)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}
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
				if (charStack.Pop() != JsonTokens.Quoter)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
                quoted = false;
            }
            currentFragment.Append(JsonTokens.Quoter);
        }

        private static JValue ParseJsonValue(string value)
        {
            JValue jsonValue;
			// quoter match is already checked.
			if (value[0] == JsonTokens.Quoter && value[value.Length - 1] == JsonTokens.Quoter)
			{
				jsonValue = new JString(value.Substring(1, value.Length - 2));
			}
			else if (value == JsonTokens.Null || value == JsonTokens.UpperCaseNull)
			{
				jsonValue = new JNull();
			}
			else if (value == JsonTokens.True || value == JsonTokens.UpperCaseTrue || value == JsonTokens.False || value == JsonTokens.UpperCaseFalse)
			{
				jsonValue = new JPrimitive(value);
			}
			else if (value.Length > 0 && (char.IsNumber(value[0]) || value[0] == JsonTokens.Negtive))
			{
				var dotIndex = value.IndexOf(JsonTokens.Dot);
				if (dotIndex == value.Length - 1 || dotIndex == 0)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
				jsonValue = new JPrimitive(value);
			}
			else
			{
				throw new ArgumentException(Messages.InvalidValue);
			}

            return jsonValue;
        }

		private static bool IsSpecial(char ch)
		{
			return ch == JsonTokens.LeftBrace || ch == JsonTokens.LeftBracket || 
					ch == JsonTokens.RightBrace || ch == JsonTokens.RightBracket || ch == JsonTokens.Comma || 
					ch == JsonTokens.Colon;
		}

    }
}
