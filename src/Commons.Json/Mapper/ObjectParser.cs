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
using System.Text;

namespace Commons.Json.Mapper
{
	public class ObjectParser : IParseEngine
	{
		private IParseEngine jsonParser;
		private IParseEngine stringParser;

		public ObjectParser(IParseEngine jsonParser, IParseEngine stringParser)
		{
			this.jsonParser = jsonParser;
			this.stringParser = stringParser;
		}

		public JValue Parse(string json)
		{
			var fragment = new StringBuilder();
			IParseEngine parser = stringParser;

			var quoted = false;
			var jsonObject = new JObject();
			var braceMatch = 0;
			var bracketMatch = 0;
			for (var pos = 0; pos < json.Length; pos++)
			{
				if (braceMatch < 0)
				{
					throw new ArgumentException();
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
					var key = parser.Parse(fragment.ToString());
					jsonObject.PutKey(key as JString);
					fragment.Clear();
					parser = jsonParser;
				}
				else if (ch.Equals(JsonTokens.Comma) && braceMatch == 1 && bracketMatch == 0)
				{
					var v = parser.Parse(fragment.ToString());
					jsonObject.PutObject(v);
					fragment.Clear();
					parser = stringParser;
				}
				else if (ch.Equals(JsonTokens.RightBrace))
				{
					--braceMatch;
					if (braceMatch == 0)
					{
						var v = parser.Parse(fragment.ToString());
						jsonObject.PutObject(v);
						if (pos < json.Length - 1)
						{
							throw new ArgumentException();
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
				throw new ArgumentException();
			}
			return jsonObject;
		}
	}
}
