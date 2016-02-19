using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Json.Mapper
{
	public class ArrayParser : IParseEngine
	{
		private JsonParseEngine jsonParseEngine;
		private IParseEngine objectParser;
		private IParseEngine stringParser;
		private IParseEngine boolParser;
		private IParseEngine numberParser;
		private IParseEngine nullParser;

		public ArrayParser(JsonParseEngine jsonParseEngine, IParseEngine objectParser, IParseEngine stringParser, 
			IParseEngine boolParser, IParseEngine numberParser, IParseEngine nullParser)
		{
			this.jsonParseEngine = jsonParseEngine;
			this.objectParser = objectParser;
			this.stringParser = stringParser;
			this.boolParser = boolParser;
			this.numberParser = numberParser;
			this.nullParser = nullParser;
		}

		public JValue Parse(string json)
		{
			throw new NotImplementedException();
		}
	}
}
