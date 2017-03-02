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

namespace Commons.Json.Mapper
{
	public sealed class JsonParseEngine
    {
        private int pos;
        private int len;
        private string json;
        public JValue Parse(string json)
        {
            this.json = json;
            len = json.Length;
            if (len < 1)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }

            var result = Parse();
			WalkThroughEmpty();
            if (pos < len)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            return result;
        }

        private JValue Parse()
        {
            WalkThroughEmpty();
			if (pos >= len)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}

            var ch = json[pos];
            switch (ch)
            {
                case JsonTokens.Digit0:
                case JsonTokens.Digit1:
                case JsonTokens.Digit2:
                case JsonTokens.Digit3:
                case JsonTokens.Digit4:
                case JsonTokens.Digit5:
                case JsonTokens.Digit6:
                case JsonTokens.Digit7:
                case JsonTokens.Digit8:
                case JsonTokens.Digit9:
                case JsonTokens.Negtive:
                    return ParseNumber();
                case JsonTokens.Quoter:
                    return ParseString();
                case JsonTokens.LeftBrace:
                    return ParseObject();
                case JsonTokens.LeftBracket:
                    return ParseArray();
                case JsonTokens.T:
                    return ParseTrue();
                case JsonTokens.F:
                    return ParseFalse();
                case JsonTokens.N:
                    return ParseNull();
                default:
                    throw new ArgumentException(Messages.InvalidFormat);
            }
        }

        private JValue ParseNumber()
        {
            var isFloat = false;
			var start = pos;
            var ch = json[pos];
			if (ch == JsonTokens.Negtive)
			{
				MovePosition();
				if (!IsDigit(json[pos]) || json[pos] == JsonTokens.Digit0)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
			}
			else if (ch == JsonTokens.Digit0)
			{
				if (pos < len - 1)
				{
					if (IsDigit(json[++pos]))
					{
						throw new ArgumentException(Messages.InvalidFormat);
					}
				}
			}

            while (pos < len && IsDigit(json[pos]))
            {
                ++pos;
            }
            if (pos < len && json[pos] == JsonTokens.Dot)
            {
                isFloat = true;
                MovePosition();
                if (!IsDigit(json[pos]))
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
            }
            while (pos < len && IsDigit(json[pos]))
            {
                pos++;
            }

            if (pos < len && char.ToLower(json[pos]) == JsonTokens.Exp)
            {
                // TODO: e+ e-
                isFloat = true;
                MovePosition();
                if (!IsDigit(json[pos]))
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                while (pos < len && IsDigit(json[pos]))
                {
                    pos++;
                }
            }

			var numStr = json.Substring(start, pos - start);

            JValue result;
            if (isFloat)
            {
                result = new JNumber(numStr, NumberType.Decimal);
            }
            else
            {
                result = new JNumber(numStr, NumberType.Integer);
            }
            return result;
        }

        private JValue ParseString()
        {
            ++pos;
			var start = pos;
            char ch;
            while (pos < len && (ch = json[pos]) != JsonTokens.Quoter)
            {
                if (ch == JsonTokens.Slash)
                {
                    //TODO: unescape char
                }
                ++pos;
            }
			var str = json.Substring(start, pos - start);
            if (pos >= len && json[pos - 1] != JsonTokens.Quoter)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            ++pos;

            return new JString(str);
        }

        private JValue ParseObject()
        {
            ++pos;
            WalkThroughEmpty();
            if (pos >= len)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            var result = new JObject();
            while (pos < len && json[pos] != JsonTokens.RightBrace)
            {
                WalkThroughEmpty();
                if (pos >= len)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                if (json[pos] != JsonTokens.Quoter)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                var key = Parse().Value;
                WalkThroughEmpty();
                if (pos >= len)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                if (json[pos] != JsonTokens.Colon)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }

                MovePosition();
                var value = Parse();
                WalkThroughEmpty();
                if (pos >= len)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                if (json[pos] != JsonTokens.Comma && json[pos] != JsonTokens.RightBrace)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                result[key] = value;
                if (json[pos] == JsonTokens.Comma)
                {
                    MovePosition();
                }
            }

            if(pos >= len && json[pos - 1] != JsonTokens.RightBrace)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            ++pos;

            return result;
        }

        private JValue ParseArray()
        {
            ++pos;
            WalkThroughEmpty();
            if (pos >= len)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }

            var result = new JArray();
            while (pos < len && json[pos] != JsonTokens.RightBracket)
            {
                var value = Parse();
                WalkThroughEmpty();
                if (pos >= len)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                if (json[pos] != JsonTokens.Comma && json[pos] != JsonTokens.RightBracket)
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
                result.Add(value);
                if (json[pos] == JsonTokens.Comma)
                {
                    MovePosition();
                }
				WalkThroughEmpty();
            }
            if (pos >= len && json[pos - 1] != JsonTokens.RightBracket)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
            ++pos;

            return result;
        }

        private JValue ParseTrue()
        {
            MovePosition();
            if (json[pos] == 'r')
            {
                MovePosition();
                if (json[pos] == 'u')
                {
                    MovePosition();
                    if (json[pos] == 'e')
                    {
                        ++pos;
                        return JBool.True;
                    }
                    else
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                }
                else
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
            }
            else
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }

        private JValue ParseFalse()
        {
            MovePosition();
            if (json[pos] == 'a')
            {
                MovePosition();
                if (json[pos] == 'l')
                {
                    MovePosition();
                    if (json[pos] == 's')
                    {
                        MovePosition();
                        if (json[pos] == 'e')
                        {
                            ++pos;
                            return JBool.False;
                        }
                        else
                        {
                            throw new ArgumentException(Messages.InvalidFormat);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                }
                else
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
            }
            else
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }

        private JValue ParseNull()
        {
            MovePosition();
            if (json[pos] == 'u')
            {
                MovePosition();
                if (json[pos] == 'l')
                {
                    MovePosition();
                    if (json[pos] == 'l')
                    {
                        ++pos;
                        return JNull.Null;
                    }
                    else
                    {
                        throw new ArgumentException(Messages.InvalidFormat);
                    }
                }
                else
                {
                    throw new ArgumentException(Messages.InvalidFormat);
                }
            }
            else
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }

        private void WalkThroughEmpty()
        {
            while (pos < len && json[pos].IsEmpty())
            {
                ++pos;
            }
        }

        private bool IsDigit(char ch)
        {
            switch (ch)
            {
                case JsonTokens.Digit0:
                case JsonTokens.Digit1:
                case JsonTokens.Digit2:
                case JsonTokens.Digit3:
                case JsonTokens.Digit4:
                case JsonTokens.Digit5:
                case JsonTokens.Digit6:
                case JsonTokens.Digit7:
                case JsonTokens.Digit8:
                case JsonTokens.Digit9:
                    return true;
                default:
                    return false;
            }
        }

        private void MovePosition()
        {
            ++pos;
            if (pos >= len)
            {
                throw new ArgumentException(Messages.InvalidFormat);
            }
        }
    }
}
