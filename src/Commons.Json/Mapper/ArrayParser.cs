﻿// Copyright CommonsForNET.
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
using System.Text;

namespace Commons.Json.Mapper
{
	internal class ArrayParser : IParseEngine
	{
		private JsonParseEngine jsonParseEngine;

		public ArrayParser(JsonParseEngine jsonParseEngine)
		{
			this.jsonParseEngine = jsonParseEngine;
		}

		public JValue Parse(string json)
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
				throw new ArgumentException();
			}

			return array;
		}

		private void AppendValue(JArray array, StringBuilder jsonFragment)
		{
			var text = jsonFragment.ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				var value = jsonParseEngine.Parse(jsonFragment.ToString());
				array.Add(value);
			}
			jsonFragment.Clear();
		}
	}
}