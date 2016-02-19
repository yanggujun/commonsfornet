using System;

namespace Commons.Json.Mapper
{
	public class NumberParser : IParseEngine
	{
		public JValue Parse(string json)
		{
			JValue value;
			var number = json.Trim();
			var dotIndex = number.IndexOf(JsonTokens.Dot);
			if (dotIndex == number.Length - 1 || dotIndex == 0)
			{
				throw new ArgumentException();
			}
			if (dotIndex < 0)
			{
				var integer = new JNumber();
				long result;
				var success = long.TryParse(number, out result);
				if (!success)
				{
					throw new ArgumentException();
				}
				integer.As(result);
				value = integer;
			}
			else
			{
				var dec = new JDecimal();
				decimal result;
				var success = decimal.TryParse(number, out result);
				if (!success)
				{
					throw new ArgumentException();
				}
				dec.As(result);
				value = dec;
			}
			return value;
		}
	}
}
