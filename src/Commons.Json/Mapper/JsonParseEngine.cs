
using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;

namespace Commons.Json.Mapper
{
	public class JsonParseEngine : IParseEngine
	{
		private const string InvalidJson = "The format of the JSON string is invalid";
		private CultureInfo culture = CultureInfo.InvariantCulture;
		private IParseEngine objectParser;
		private IParseEngine arrayParser;
		private IParseEngine stringParser;
		private IParseEngine boolParser;
		private IParseEngine numberParser;
		private IParseEngine nullParser;

		public JsonParseEngine()
		{
			stringParser = new StringParser();
			boolParser = new BoolParser();
			numberParser = new NumberParser();
			nullParser = new NullParser();
			objectParser = new ObjectParser(this, arrayParser, stringParser, boolParser, numberParser, nullParser);
			arrayParser = new ArrayParser(this, objectParser, stringParser, boolParser, numberParser, nullParser);
		}

		public JValue Parse(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentException(InvalidJson);
			}
			var cleaned = json.Trim();
			var firstCh = cleaned[0];
			IParseEngine parser;
			if (firstCh.Equals(JsonTokens.LeftBrace))
			{
				parser = objectParser;
			}
			else if (firstCh.Equals(JsonTokens.LeftBracket))
			{
				parser = arrayParser;
			}
			else if (firstCh.Equals(JsonTokens.Quoter))
			{
				parser = stringParser;
			}
			else if (firstCh.Equals('N') || firstCh.Equals('n'))
			{
				parser = nullParser;
			}
			else if (firstCh.Equals('T') || firstCh.Equals('t') 
				|| firstCh.Equals('F') || firstCh.Equals('f'))
			{
				parser = boolParser;
			}
			else if (Char.IsNumber(firstCh) || firstCh.Equals('-'))
			{
				parser = numberParser;
			}
			else
			{
				throw new ArgumentException(InvalidJson);
			}

			return parser.Parse(cleaned);
		}

	}
}
