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
using System.Globalization;

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
			objectParser = new ObjectParser(this, stringParser);
			arrayParser = new ArrayParser(this);
		}

		public JValue Parse(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentException(InvalidJson);
			}
			var text = json.Trim();
			var firstCh = text[0];
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

			return parser.Parse(text);
		}

	}
}
