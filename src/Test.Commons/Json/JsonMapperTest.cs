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
using Commons.Json;
using Commons.Json.Mapper;
using Xunit;

namespace Test.Commons.Json
{
	public class JsonMapperTest
	{
		[Fact]
		public void TestJsonObjectMapper()
		{
			var obj = JsonMapper.ToObject<ComplexPoco>(@"{fieldA : 'valueA'}");
			Assert.Equal(obj.FieldA, "valueA");
		}

		[Fact]
		public void TestToJsonNormalObject()
		{
			var json = JsonMapper.ToJson(new {FieldA = "AValue", FieldB = "BValue"});
			dynamic jsonObject = JsonObject.Parse(json);
			Assert.Equal("AValue", jsonObject.FieldA);
			Assert.Equal("BValue", jsonObject.FieldB);
		}

		[Fact]
		public void TestToJsonSimpleArray()
		{
			var json = JsonMapper.ToJson(new[] {1, 2, 3, 4, 5, 6});
			dynamic jsonObject = JsonObject.Parse(json);
			Assert.Equal(1, jsonObject[0]);
		}

		[Fact]
		public void TestToJsonComplexArray()
		{
			var json = JsonMapper.ToJson(new object[] {1, "a string", 6.04, new {FieldA = "ValueA", FieldB = "ValueB"}});
			dynamic jsonObject = JsonObject.Parse(json);
			Assert.Equal(1, jsonObject[1]);
			Assert.Equal("a string", jsonObject[1]);
			Assert.Equal(6.04, jsonObject[2], 0.0001);
			Assert.Equal("ValueA", jsonObject[3].FieldA);
			Assert.Equal("ValueB", jsonObject[3].FieldB);
		}

		[Fact]
		public void TestParseEngine1()
		{
			var json = "{\"name\": \"Jack\"}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
		}

		[Fact]
		public void TestParseEngine2()
		{
			var json = "{\"nam\"e\": \"Jack\"}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine3()
		{
			var json = "{\"nam\"e\": \"Jack\"}{";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine4()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\"}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
		}

		[Fact]
		public void TestParseEngine5()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
		}

		[Fact]
		public void TestParseEngine6()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30.}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine7()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": .30}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine8()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": -30}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
		}

		[Fact]
		public void TestParseEngine9()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": -30.5}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
		}
	}
}
