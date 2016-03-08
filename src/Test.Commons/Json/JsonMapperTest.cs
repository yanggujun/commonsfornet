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
using System.Collections.Generic;
using System.Diagnostics;
using Commons.Json;
using Commons.Json.Mapper;
using Xunit;

namespace Test.Commons.Json
{
	public class JsonMapperTest
	{
		[Fact]
		public void TestJsonObjectMapper01()
		{
			var obj = JsonMapper.ToObject<Simple>("{\"FieldA\": \"valueA\", \"FieldB\" : 10, \"FieldC\": 2.3, \"FieldD\": true}");
			Assert.Equal(obj.FieldA, "valueA");
            Assert.Equal(obj.FieldB, 10);
            Assert.Equal(obj.FieldC, 2.3, 2);
            Assert.True(obj.FieldD);
		}

		[Fact]
		public void TestJsonObjectMapper02()
		{
			var json = "{\"fieldE\": \"valueE\", \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": 10, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true}";
			var nested = JsonMapper.ToObject<Nested>(json);
			Assert.Equal(nested.FieldE, "valueE");
			Assert.Equal(nested.FieldF, 20);
			Assert.Equal(nested.FieldG, 3.459, 4);
			Assert.True(nested.FieldH);
			Assert.NotNull(nested.Simple);
			Assert.Equal(nested.Simple.FieldA, "valueA");
			Assert.Equal(nested.Simple.FieldB, 10);
			Assert.Equal(nested.Simple.FieldC, 1.2997, 5);
			Assert.False(nested.Simple.FieldD);
		}

		[Fact]
		public void TestJsonObjectMapper03()
		{
			var json = "{\"fieldE\": \"valueE\", \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": 10, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.ToObject<Nested>(json);
			Assert.Equal(nested.FieldE, "valueE");
			Assert.Equal(nested.FieldF, 20);
			Assert.Equal(nested.FieldG, 3.459, 4);
			Assert.True(nested.FieldH);
			Assert.NotNull(nested.Simple);
			Assert.Equal(nested.Simple.FieldA, "valueA");
			Assert.Equal(nested.Simple.FieldB, 10);
			Assert.Equal(nested.Simple.FieldC, 1.2997, 5);
			Assert.False(nested.Simple.FieldD);
		}

		[Fact]
		public void TestJsonObjectMapper04()
		{
			var json = "{\"fielde\": null, \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": 10, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.ToObject<Nested>(json);
			Assert.Null(nested.FieldE);
			Assert.Equal(nested.FieldF, 20);
			Assert.Equal(nested.FieldG, 3.459, 4);
			Assert.True(nested.FieldH);
			Assert.NotNull(nested.Simple);
			Assert.Equal(nested.Simple.FieldA, "valueA");
			Assert.Equal(nested.Simple.FieldB, 10);
			Assert.Equal(nested.Simple.FieldC, 1.2997, 5);
			Assert.False(nested.Simple.FieldD);
			
		}

		[Fact]
		public void TestJsonObjectMapper05()
		{
			var json = "{\"fielde\": null, \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.ToObject<Nested>(json);
			Assert.Null(nested.FieldE);
			Assert.Equal(nested.FieldF, 20);
			Assert.Equal(nested.FieldG, 3.459, 4);
			Assert.True(nested.FieldH);
			Assert.NotNull(nested.Simple);
			Assert.Equal(nested.Simple.FieldA, "valueA");
			Assert.Equal(nested.Simple.FieldB, 0);
			Assert.Equal(nested.Simple.FieldC, 1.2997, 5);
			Assert.False(nested.Simple.FieldD);
		}

		[Fact]
		public void TestJsonObjectMapper06()
		{
			var json = "{\"fielde\": 33, \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.ToObject<Nested>(json));
		}

		[Fact]
		public void TestJsonObjectMapper07()
		{
			var json = "{\"fielde\": \"valueE\", \"fieldF\": 20, \"FieldG\": 3, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.ToObject<Nested>(json);
			Assert.Equal(3, nested.FieldG, 1);
		}

		[Fact]
		public void TestJsonObjectMapper08()
		{
			var json = "{\"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.ToObject<Nested>(json);
			Assert.Null(nested.FieldE);
			Assert.Equal(nested.FieldF, 20);
			Assert.Equal(nested.FieldG, 3.459, 4);
			Assert.True(nested.FieldH);
			Assert.NotNull(nested.Simple);
			Assert.Equal(nested.Simple.FieldA, "valueA");
			Assert.Equal(nested.Simple.FieldB, 0);
			Assert.Equal(nested.Simple.FieldC, 1.2997, 5);
			Assert.False(nested.Simple.FieldD);
		}

		[Fact]
		public void TestJsonObjectMapper09()
		{
			var json = "{\"fieldF\": 20.543, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.ToObject<Nested>(json));
		}

        [Fact]
        public void TestJsonObjectMapper10()
        {
            var json = "[{\"FieldA\": \"valueA\", \"FieldB\" : 10, \"FieldC\": 2.3, \"FieldD\": true}, " 
                        + "{\"FieldA\": \"valueA1\", \"FieldB\" : 11, \"FieldC\": 3.3, \"FieldD\": false}]";
            var list = JsonMapper.ToObject<List<Simple>>(json);
            Assert.Equal(2, list.Count);
            Assert.Equal("valueA", list[0].FieldA);
            Assert.Equal(10, list[0].FieldB);
            Assert.Equal(2.3, list[0].FieldC, 2);
            Assert.True(list[0].FieldD);
            Assert.Equal("valueA1", list[1].FieldA);
            Assert.Equal(11, list[1].FieldB);
            Assert.Equal(3.3, list[1].FieldC, 2);
            Assert.False(list[1].FieldD);
        }

        [Fact]
        public void TestJsonObjectMapper11()
        {
            var json = "[1, 2, 3, 4, 5, 6, 7, 8]";
            var list = JsonMapper.ToObject<List<int>>(json);
            for (var i = 0; i < list.Count; i++)
            {
                Assert.Equal(i + 1, list[i]);
            }
        }

		[Fact]
		public void TestToJsonNormalObject()
		{
			var json = JsonMapper.ToJson(new {FieldA = "AValue", FieldB = "BValue"});
			dynamic jsonObject = JsonMapper.Parse(json);
			Assert.Equal("AValue", jsonObject.FieldA);
			Assert.Equal("BValue", jsonObject.FieldB);
		}

		[Fact]
		public void TestToJsonSimpleArray()
		{
			var json = JsonMapper.ToJson(new[] {1, 2, 3, 4, 5, 6});
			dynamic jsonObject = JsonMapper.Parse(json);
			Assert.Equal(1, jsonObject[0]);
		}

		[Fact]
		public void TestToJsonComplexArray()
		{
			var json = JsonMapper.ToJson(new object[] {1, "a string", 6.04, new {FieldA = "ValueA", FieldB = "ValueB"}});
			dynamic jsonObject = JsonMapper.Parse(json);
			Assert.Equal(1, jsonObject[1]);
			Assert.Equal("a string", jsonObject[1]);
			Assert.Equal(6.04, jsonObject[2], 0.0001);
			Assert.Equal("ValueA", jsonObject[3].FieldA);
			Assert.Equal("ValueB", jsonObject[3].FieldB);
		}

		[Fact]
		public void TestParseEngine01()
		{
			var json = "{\"name\": \"Jack\"}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = (JObject) value;
			Assert.Equal("Jack", ((JString) obj[new JString("name")]).Value);
			Assert.Equal("Jack", obj[new JString("name")] as JString);
		}

		[Fact]
		public void TestParseEngine02()
		{
			var json = "{\"nam\"e\": \"Jack\"}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine03()
		{
			var json = "{\"nam\"e\": \"Jack\"}{";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine04()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\"}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.Equal("usa", obj["country"] as JString);
		}

		[Fact]
		public void TestParseEngine05()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.Equal(30, (int) (obj["age"] as JInteger));
		}

		[Fact]
		public void TestParseEngine06()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30.}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine07()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": .30}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine08()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": -30}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.Equal(-30, (int) (obj["age"] as JInteger));
		}

		[Fact]
		public void TestParseEngine09()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": -30.5}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.Equal(-30.5, (obj["age"] as JDecimal).AsDouble());
		}

		[Fact]
		public void TestParseEngine10()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 3f0}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine11()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": false}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.False(obj["married"] as JBoolean);
		}

		[Fact]
		public void TestParseEngine12()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": true}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.True(obj["married"] as JBoolean);
		}

		[Fact]
		public void TestParseEngine13()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": tRue}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.True(obj["married"] as JBoolean);
		}

		[Fact]
		public void TestParseEngine14()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": faLse}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json);
			Assert.True(value is JObject);
			var obj = value as JObject;
			Assert.False(obj["married"] as JBoolean);
		}

		[Fact]
		public void TestParseEngine15()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": fase}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine16()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": \"fase}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine17()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": false, \"child\": null}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JObject;
			Assert.True(value["child"] is JNull);
		}

		[Fact]
		public void TestParseEngine18()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": false, \"child\": Null}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JObject;
			Assert.True(value["child"] is JNull);
		}

		[Fact]
		public void TestParseEngine19()
		{
			var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": false, \"child\": Nill}";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine20()
		{
			var json = "\"Jason\"";
			var parseEngine = new JsonParseEngine();
			Assert.Equal("Jason", parseEngine.Parse(json) as JString);
		}

		[Fact]
		public void TestParseEngine21()
		{
			var json = "20";
			var parseEngine = new JsonParseEngine();
			Assert.Equal(20, (parseEngine.Parse(json) as JInteger).AsInt());
		}

		[Fact]
		public void TestParseEngine22()
		{
			var json = "21.0002345";
			var parseEngine = new JsonParseEngine();
			Assert.Equal(21.0002345, (parseEngine.Parse(json) as JDecimal).AsDouble());
		}

		[Fact]
		public void TestParseEngine23()
		{
			var json = "{{ \"name\": \"Jack\"";
			var parseEngine = new JsonParseEngine();
			Assert.Throws(typeof (ArgumentException), () => parseEngine.Parse(json));
		}

		[Fact]
		public void TestParseEngine24()
		{
			var json = "[\"Jack\", \"John\", \"Jose\", \"Jame\"]";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JArray;
			Assert.NotNull(value);
			Assert.Equal("Jack", value[0] as JString);
			Assert.Equal("John", value[1] as JString);
			Assert.Equal("Jose", value[2] as JString);
			Assert.Equal("Jame", value[3] as JString);
		}

		[Fact]
		public void TestParseEngine25()
		{
			var json = "[\"Jack\", [\"Sam\", \"Jon\"], \"Tom\"]";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JArray;
			Assert.NotNull(value);
			Assert.Equal("Jack", value[0] as JString);
			Assert.Equal("Sam", (value[1] as JArray)[0] as JString);
			Assert.Equal("Jon", (value[1] as JArray)[1] as JString);
			Assert.Equal("Tom", value[2] as JString);
		}

		[Fact]
		public void TestParseEngine26()
		{
			var json = "{\"name\": \"Jack\", \"education\": { \"college\": \"nyu\", \"graduate\": \"mit\"}}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JObject;
			Assert.NotNull(value);
			var edu = value["education"] as JObject;
			Assert.Equal("nyu", edu["college"] as JString);
			Assert.Equal("mit", edu["graduate"] as JString);
		}

		[Fact]
		public void TestParseEngine27()
		{
			var json = "[{\"Jack\": {\"age\": 27, \"country\": \"usa\"}}, {\"Jon\": {\"age\": 21, \"country\" : \"canada\"}}]";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JArray;
			Assert.NotNull(value);
			var jack = (value[0] as JObject)["Jack"] as JObject;
			Assert.NotNull(jack);
			Assert.Equal(27, (jack["age"] as JInteger).AsInt());
			Assert.Equal("usa", jack["country"] as JString);
			var jon = (value[1] as JObject)["Jon"] as JObject;
			Assert.NotNull(jon);
			Assert.Equal(21, (jon["age"] as JInteger).AsInt());
			Assert.Equal("canada", jon["country"] as JString);
		}

		[Fact]
		public void TestParseEngine28()
		{
			var json =
				"{\"english\": {\"top\": [\"arsenal\", \"man utd\", \"chelsea\", \"man city\"], \"holder\": \"chelsea\"}, \"spain\": {\"top\": [\"barcelona\", \"real\"], \"holder\": \"barcelona\"}}";
			var parseEngine = new JsonParseEngine();
			var value = parseEngine.Parse(json) as JObject;
			Assert.NotNull(value);
			var english = value["english"] as JObject;
			Assert.NotNull(english);
			var englishTop = english["top"] as JArray;
			Assert.NotNull(englishTop);
			Assert.Equal("arsenal", englishTop[0] as JString);
			Assert.Equal("man utd", englishTop[1] as JString);
			Assert.Equal("chelsea", englishTop[2] as JString);
			Assert.Equal("man city", englishTop[3] as JString);
			Assert.Equal("chelsea", english["holder"] as JString);
			var spain = value["spain"] as JObject;
			Assert.NotNull(spain);
			var spainTop = spain["top"] as JArray;
			Assert.NotNull(spainTop);
			Assert.Equal("barcelona", spainTop[0] as JString);
			Assert.Equal("real", spainTop[1] as JString);
			Assert.Equal("barcelona", spain["holder"] as JString);
		}

		[Fact]
		public void TestParseEngine29()
		{
			var engine = new JsonParseEngine();
			var json4 = TestHelper.ReadFrom(@".\Json\JsonSample4.txt");
			engine.Parse(json4);
			var json5 = TestHelper.ReadFrom(@".\Json\JsonSample5.txt");
			engine.Parse(json5);
			var json6 = TestHelper.ReadFrom(@".\Json\JsonSample6.txt");
			engine.Parse(json6);
			var json7 = TestHelper.ReadFrom(@".\Json\JsonSample7.txt");
			engine.Parse(json7);
			var json8 = TestHelper.ReadFrom(@".\Json\JsonSample8.txt");
			engine.Parse(json8);
			var json9 = TestHelper.ReadFrom(@".\Json\JsonSample10.txt");
			engine.Parse(json9);
		}

		[Fact]
		public void TestParseEngine30()
		{
			var engine = new JsonParseEngine();
			var json = "[]";
			var array = engine.Parse(json) as JArray;
			Assert.Equal(0, array.Length);
		}

		[Fact]
		public void TestParseEngine31()
		{
			var engine = new JsonParseEngine();
			var json = "{}";
			var obj = engine.Parse(json) as JObject;
			Assert.NotNull(obj);
			var valueCount = 0;
			foreach (var item in obj)
			{
				valueCount++;
			}
			Assert.Equal(0, valueCount);
		}

		[Fact]
		public void TestParseEngine32()
		{
			var engine = new JsonParseEngine();
			var json = "{\"a\": }";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine33()
		{
			var engine = new JsonParseEngine();
			var json = "[{}]";
			var array = engine.Parse(json) as JArray;
			Assert.NotNull(array);
			var obj = array[0] as JObject;
			Assert.NotNull(obj);
			foreach (var item in obj)
			{
				Assert.Equal(0, 1);
			}
		}

        [Fact]
        public void TestParseEngine34()
        {
            var engine = new JsonParseEngine();
            var json = "{\"a\",}";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine35()
        {
            var engine = new JsonParseEngine();
            var json = "{,}";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine36()
        {
            var engine = new JsonParseEngine();
            var json = "[,]";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine37()
        {
            var engine = new JsonParseEngine();
            var json = "[,\"a\"]";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine38()
        {
            var engine = new JsonParseEngine();
            var json = "[\"a\", ]";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine39()
        {
            var engine = new JsonParseEngine();
            var json = "{\"a\"}";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

		[Fact]
		public void TestParseEngine40()
		{
			var engine = new JsonParseEngine();
			var json = "{\"a\",,}";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine41()
		{
			var engine = new JsonParseEngine();
			var json = "[[[[[[null]]]]]]]]]]";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine42()
		{
			var engine = new JsonParseEngine();
			var json = "[{}, {},{}}]";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine43()
		{
			var engine = new JsonParseEngine();
			var json = ",";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine44()
		{
			var engine = new JsonParseEngine();
			var json = "{,\"a\" : \"b\",}";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine45()
		{
			var engine = new JsonParseEngine();
			var json = "[,\"a\", ]";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		[Fact]
		public void TestParseEngine46()
		{
			var engine = new JsonParseEngine();
			var json = "{\"a\": \"b\", \"c\"}";
			Assert.Throws(typeof (ArgumentException), () => engine.Parse(json));
		}

		public void TestPerformance()
		{
			var engine = new JsonParseEngine();
			var json7 = TestHelper.ReadFrom(@".\Json\JsonSample10.txt");
			engine.Parse(json7);
			var sw = new Stopwatch();
			sw.Start();
			for (var i = 0; i < 10000; i++)
			{
				var obj = engine.Parse(json7) as JObject;
				Assert.Equal("Alpha", obj["a"] as JString);
			}
			sw.Stop();
			Console.WriteLine("Parse Engine: " + sw.ElapsedMilliseconds);
			sw.Reset();
			JsonMapper.Parse(json7);
			sw.Start();
			for (var i = 0; i < 10000; i++)
			{
				dynamic obj = JsonMapper.Parse(json7);
				Assert.Equal("Alpha", (string)obj["a"]);
			}
			sw.Stop();
			Console.WriteLine("Dynamic Object: " + sw.ElapsedMilliseconds);
		}
	}
}
