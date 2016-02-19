using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.Json.Mapper
{
	public class ObjectParser : IParseEngine
	{
		private IParseEngine jsonParser;
		private IParseEngine arrayParser;
		private IParseEngine stringParser;
		private IParseEngine boolParser;
		private IParseEngine numberParser;
		private IParseEngine nullParser;

		public ObjectParser(IParseEngine jsonParser, IParseEngine arrayParser, IParseEngine stringParser,
			IParseEngine boolParser, IParseEngine numberParser, IParseEngine nullParser)
		{
			this.jsonParser = jsonParser;
			this.arrayParser = arrayParser;
			this.stringParser = stringParser;
			this.boolParser = boolParser;
			this.numberParser = numberParser;
			this.nullParser = nullParser;
		}

		public JValue Parse(string json)
		{
			var fragment = new StringBuilder();
			IParseEngine parser = stringParser;

			var quoted = false;
			var jsonObject = new JObject();
			var braceMatch = 0;
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
					fragment.Clear();
				}
				else if (ch.Equals(JsonTokens.Colon))
				{
					var key = parser.Parse(fragment.ToString());
					jsonObject.PutKey(key as JString);
					fragment.Clear();
					parser = jsonParser;
				}
				else if (ch.Equals(JsonTokens.Comma))
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
				}
				else
				{
					fragment.Append(ch);
				}
			}
			if (quoted)
			{
				throw new ArgumentException();
			}
			return jsonObject;
		}
	}
}
