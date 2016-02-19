using System;

namespace Commons.Json.Mapper
{
	public class StringParser : IParseEngine
	{
		public JValue Parse(string json)
		{
			var value = json.Trim();
			if (!value[0].Equals(JsonTokens.Quoter) || !value[value.Length - 1].Equals(JsonTokens.Quoter))
			{
				throw new ArgumentException();
			}
			var str = value.Trim(JsonTokens.Quoter);
			return new JString(str);
		}
	}
}
