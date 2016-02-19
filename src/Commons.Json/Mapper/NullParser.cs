using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Json.Mapper
{
	public class NullParser : IParseEngine
	{
		public JValue Parse(string json)
		{
			throw new NotImplementedException();
		}
	}
}
