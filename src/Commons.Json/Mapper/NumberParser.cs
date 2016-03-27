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
	internal class NumberParser : IParseEngine
	{
		public JValue Parse(string json)
		{
			JValue value;
			var number = json.Trim();
			var dotIndex = number.IndexOf(JsonTokens.Dot);
			if (dotIndex == number.Length - 1 || dotIndex == 0)
			{
				throw new ArgumentException(Messages.InvalidFormat);
			}
			if (dotIndex < 0)
			{
				long result;
				var success = long.TryParse(number, out result);
				if (!success)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
				var integer = new JInteger(result);
				value = integer;
			}
			else
			{
				decimal result;
				var success = decimal.TryParse(number, out result);
				if (!success)
				{
					throw new ArgumentException(Messages.InvalidFormat);
				}
				var dec = new JDecimal(result);
				value = dec;
			}
			return value;
		}
	}
}
