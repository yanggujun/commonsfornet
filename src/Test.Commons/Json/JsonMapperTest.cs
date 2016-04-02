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
using System.Globalization;
using Commons.Collections.Map;
using Commons.Collections.Set;
using Commons.Json;
using Commons.Json.Mapper;
using Xunit;

namespace Test.Commons.Json
{
	public class JsonMapperTest
	{
		/// <summary>
		/// plain object
		/// </summary>
		[Fact]
		public void TestMapJsonToObject01()
		{
			var obj = JsonMapper.To<Simple>("{\"FieldA\": \"valueA\", \"FieldB\" : 10, \"FieldC\": 2.3, \"FieldD\": true}");
			Assert.Equal(obj.FieldA, "valueA");
            Assert.Equal(obj.FieldB, 10);
            Assert.Equal(obj.FieldC, 2.3, 2);
            Assert.True(obj.FieldD);
		}

		/// <summary>
		/// complex object.
		/// </summary>
		[Fact]
		public void TestMapJsonToObject02()
		{
			var json = "{\"fieldE\": \"valueE\", \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": 10, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true}";
			var nested = JsonMapper.To<Nested>(json);
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
		public void TestMapJsonToObject03()
		{
			var json = "{\"fieldE\": \"valueE\", \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": 10, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.To<Nested>(json);
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
		public void TestMapJsonToObject04()
		{
			var json = "{\"fielde\": null, \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": 10, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.To<Nested>(json);
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
		public void TestMapJsonToObject05()
		{
			var json = "{\"fielde\": null, \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.To<Nested>(json);
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
		public void TestMapJsonToObject06()
		{
			var json = "{\"fielde\": 33, \"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<Nested>(json));
		}

		[Fact]
		public void TestMapJsonToObject07()
		{
			var json = "{\"fielde\": \"valueE\", \"fieldF\": 20, \"FieldG\": 3, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.To<Nested>(json);
			Assert.Equal(3, nested.FieldG, 1);
		}

		[Fact]
		public void TestMapJsonToObject08()
		{
			var json = "{\"fieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.To<Nested>(json);
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
		public void TestMapJsonToObject09()
		{
			var json = "{\"fieldF\": 20.543, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
			           + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<Nested>(json));
		}

        [Fact]
        public void TestMapJsonToObject10()
        {
            var json = "[{\"FieldA\": \"valueA\", \"FieldB\" : 10, \"FieldC\": 2.3, \"FieldD\": true}, " 
                        + "{\"FieldA\": \"valueA1\", \"FieldB\" : 11, \"FieldC\": 3.3, \"FieldD\": false}]";
            var list = JsonMapper.To<List<Simple>>(json);
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
        public void TestMapJsonToObject11()
        {
            var json = "[1, 2, 3, 4, 5, 6, 7, 8]";
            var list = JsonMapper.To<List<int>>(json);
            for (var i = 0; i < list.Count; i++)
            {
                Assert.Equal(i + 1, list[i]);
            }
        }

		[Fact]
		public void TestMapJsonToObject12()
		{
			var json = "{\"fieldj\": \"valueJ\", \"fieldk\": [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20]}";
			var primitiveList = JsonMapper.To<PrimitiveList>(json);
			Assert.Equal("valueJ", primitiveList.FieldJ);
			for (var i = 0; i < primitiveList.FieldK.Count; i++)
			{
				Assert.Equal(i + 10, primitiveList.FieldK[i]);
			}
		}

		[Fact]
		public void TestMapJsonToObject13()
		{
			var json = TestHelper.ReadFrom(@".\Json\JsonSample11.txt");
			var arrayNested = JsonMapper.To<ArrayNested>(json);
			Assert.Equal("valueI", arrayNested.FieldI);
			Assert.NotNull(arrayNested.NestedItems);
			Assert.Equal(2, arrayNested.NestedItems.Count);
			Assert.Equal("valueE1", arrayNested.NestedItems[0].FieldE);
			Assert.Equal(100, arrayNested.NestedItems[0].FieldF);
			Assert.Equal(1000.2345, arrayNested.NestedItems[0].FieldG);
			Assert.True(arrayNested.NestedItems[0].FieldH);
			Assert.NotNull(arrayNested.NestedItems[0].Simple);
			Assert.Equal("valueA1", arrayNested.NestedItems[0].Simple.FieldA);
			Assert.Equal(200, arrayNested.NestedItems[0].Simple.FieldB);
			Assert.Equal(10000.35985, arrayNested.NestedItems[0].Simple.FieldC);
			Assert.False(arrayNested.NestedItems[0].Simple.FieldD);

			Assert.Equal("valueE2", arrayNested.NestedItems[1].FieldE);
			Assert.Equal(300, arrayNested.NestedItems[1].FieldF);
			Assert.Equal(2000.2345, arrayNested.NestedItems[1].FieldG);
			Assert.False(arrayNested.NestedItems[1].FieldH);
			Assert.NotNull(arrayNested.NestedItems[1].Simple);
			Assert.Equal("valueA2", arrayNested.NestedItems[1].Simple.FieldA);
			Assert.Equal(400, arrayNested.NestedItems[1].Simple.FieldB);
			Assert.Equal(20000.35985, arrayNested.NestedItems[1].Simple.FieldC);
			Assert.True(arrayNested.NestedItems[1].Simple.FieldD);

		}

		[Fact]
		public void TestMapJsonToObject14()
		{
			var json = "\"astring\"";
			var str = JsonMapper.To<string>(json);
			Assert.Equal("astring", str);
		}

		[Fact]
		public void TestMapJsonToObject15()
		{
			var json = "10";
			var integer = JsonMapper.To<int>(json);
			Assert.Equal(10, integer);
		}

		[Fact]
		public void TestMapJsonToObject16()
		{
			var json = "10.44323";
			var floating = JsonMapper.To<double>(json);
			Assert.Equal(10.44323, floating);
		}

        [Fact]
        public void TestMapJsonToObject161()
        {
			var json = "10.44323";
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<int>(json));
        }

        [Fact]
        public void TestMapJsonToObject162()
        {
            var json = "10";
            var number = JsonMapper.To<double>(json);
            Assert.Equal(10, number);
        }

        [Fact]
        public void TestMapJsonToObject163()
        {
            var json = "10";
            var number = JsonMapper.To<decimal>(json);
            Assert.Equal(10, number);
        }

        [Fact]
        public void TestMapJsonToObject164()
        {
            var json = "11";
            var number = JsonMapper.To<float>(json);
            Assert.Equal(11, number);
        }

		[Fact]
		public void TestMapJsonToObject17()
		{
			var json = "true";
			var boolean = JsonMapper.To<bool>(json);
			Assert.True(boolean);
		}

		[Fact]
		public void TestMapJsonToObject18()
		{
			var json = "null";
			var simple = JsonMapper.To<Simple>(json);
			Assert.Null(simple);
		}

		[Fact]
		public void TestMapJsonToObject19()
		{
			var json = "daddaaadaa";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<bool>(json));
		}

		[Fact]
		public void TestMapJsonToObject20()
		{
			var json = "1.4";
			var number = JsonMapper.To<float>(json);
			Assert.Equal(1.4, number, 2);
		}

		[Fact]
		public void TestMapJsonToObject21()
		{
			var json = "5";
			var number = JsonMapper.To<short>(json);
			Assert.Equal(5, number);
		}

        [Fact]
        public void TestMapJsonToObject211()
        {
            var json = "10";
            var number = JsonMapper.To<byte>(json);
            Assert.Equal(10, number);
        }

        [Fact]
        public void TestMapJsonToObject212()
        {
            var json = "10";
            var number = JsonMapper.To<sbyte>(json);
            Assert.Equal(10, number);
        }

		[Fact]
		public void TestMapJsonToObject22()
		{
			var json = "6";
			var number = JsonMapper.To<uint>(json);
			Assert.Equal((uint)6, number);
		}

		[Fact]
		public void TestMapJsonToObject23()
		{
			var json = "-1000";
			var number = JsonMapper.To<int>(json);
			Assert.Equal(-1000, number);
		}

		[Fact]
		public void TestMapJsonToObject24()
		{
			var json = "7";
			var number = JsonMapper.To<ushort>(json);
			Assert.Equal((ushort)7, number);
		}

		[Fact]
		public void TestMapJsonToObject25()
		{
			var json = "800000";
			var number = JsonMapper.To<long>(json);
			Assert.Equal(800000, number);
		}

		[Fact]
		public void TestMapJsonToObject26()
		{
			var json = "800000";
			var number = JsonMapper.To<ulong>(json);
			Assert.Equal((ulong)800000, number);
		}

        [Fact]
        public void TestMapJsonToObject27()
        {
            var json = "{\"fieldb\": 10}";
            var simple = JsonMapper.To<Simple>(json);
            Assert.Equal(10, simple.FieldB);
            Assert.Null(simple.FieldA);
            Assert.Equal(0, simple.FieldC);
            Assert.False(simple.FieldD);
        }

        [Fact]
        public void TestMapJsonToObject28()
        {
            var json = "{\"fieldb\": 10, \"NotExistInObject\": \"aaaa\"}";
            var simple = JsonMapper.To<Simple>(json);
            Assert.Equal(10, simple.FieldB);
            Assert.Null(simple.FieldA);
            Assert.Equal(0, simple.FieldC);
            Assert.False(simple.FieldD);
        }

        [Fact]
        public void TestMapJsonToObject29()
        {
            var json = "{\"fieldl\":\"valuel\", \"fieldm\": [1, 2, 3, 4 ,5 ,6]}";
            var setNested = JsonMapper.To<SetNested>(json);
            Assert.Equal("valuel", setNested.FieldL);
            Assert.NotNull(setNested.FieldM);
            Assert.Equal(0, setNested.FieldM.Count);
        }

        [Fact]
        public void TestMapJsonToObject30()
        {
            var json = "{\"fieldl\": 10, \"fieldm\": [1, 2, 3, 4 ,5 ,6]}";
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<SetNested>(json));
        }

        [Fact]
        public void TestMapJsonToObject31()
        {
            var json = "{\"fieldl\": {}, \"fieldm\": [1, 2, 3, 4 ,5 ,6]}";
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<SetNested>(json));
        }

        [Fact]
        public void TestMapJsonToObject32()
        {
            var json = "{\"Birthday\": \"1990/01/18\", \"name\": \"alan\"}";
            var hasDate = JsonMapper.UseDateFormat(string.Empty).To<HasDate>(json);
            Assert.Equal("alan", hasDate.Name);
            Assert.Equal(1990, hasDate.Birthday.Year);
            Assert.Equal(1, hasDate.Birthday.Month);
            Assert.Equal(18, hasDate.Birthday.Day);
        }

        [Fact]
        public void TestMapJsonToObject33()
        {
            var json = "{\"Birthday\": \"1990/1/5/88\", \"name\": \"alan\"}";
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.UseDateFormat(string.Empty).To<HasDate>(json));
        }

		[Fact]
		public void TestMapJsonToObject34()
		{
			var json = "{\"Name\": \"Ben\", \"Numbers\": [0, 1, 2, 4, 5]}";
			var ilistNested = JsonMapper.To<IListNested>(json);
			Assert.Equal("Ben", ilistNested.Name);
			Assert.Null(ilistNested.Numbers);
		}

        [Fact]
        public void TestMapJsonToObject35()
        {
            var json = "{\"Name\": \"Catherine\", \"Gender\": \"F\", \"Country\": \"UK\", \"Office\": \"London\"}";
            var dict = JsonMapper.To<Dictionary<string, string>>(json);
            Assert.Equal("Catherine", dict["Name"]);
            Assert.Equal("F", dict["Gender"]);
            Assert.Equal("UK", dict["Country"]);
            Assert.Equal("London", dict["Office"]);
        }

        [Fact]
        public void TestMapJsonToObject36()
        {
            var json = "{\"Name\": \"Alan\", \"Age\": 25, \"Score\": 90.5, \"ExamDate\": \"2016/03/01\", \"Pass\": true}";
            var simple = JsonMapper.UseDateFormat(string.Empty).To<SimpleStruct>(json);
            Assert.Equal("Alan", simple.Name);
            Assert.Equal(25, simple.Age);
            Assert.Equal(90.5, simple.Score);
            Assert.Equal(2016, simple.ExamDate.Year);
            Assert.Equal(3, simple.ExamDate.Month);
            Assert.Equal(1, simple.ExamDate.Day);
            Assert.True(simple.Pass);
        }

		[Fact]
		public void TestMapJsonToObject37()
		{
			var json = "{\"Name\": \"John\", \"Age\": 10}";
			var obj = JsonMapper.To<PrivateSetter>(json);
			Assert.Equal("John", obj.Name);
			Assert.Equal(23, obj.Age);
		}

		[Fact]
		public void TestMapJsonToObject38()
		{
			var json = "{\"Name\": \"Kevin\", \"Age\": 21}";
			var obj = JsonMapper.To<PrivateGetter>(json);
			Assert.Equal("Kevin", obj.Name);
			Assert.Equal(21, obj.ActualAge());
		}

        [Fact]
        public void TestMapJsonToObject39()
        {
            var json = "{\"Name\": \"Kevin\", \"Age\": 23}";
            Assert.Throws(typeof(InvalidOperationException), () => JsonMapper.To<NoDefaultConstructor>(json));
        }

		[Fact]
		public void TestMapJsonToObject40()
		{
			var json = "{\"fieldj\": \"valuej\", \"fieldk\": [1, 2, 4a5, 6]}";
			Assert.Throws(typeof (ArgumentException), () => JsonMapper.To<PrimitiveList>(json));
		}

		[Fact]
		public void TestMapJsonToObject41()
		{
			var json = "\"Art\"";
			var major = JsonMapper.To<Major>(json);
			Assert.Equal(Major.Art, major);
		}

		[Fact]
		public void TestMapJsonToObject42()
		{
			var json = "[0, 1, 2, 3, 4, 5, 6]";
			var array = JsonMapper.To<int[]>(json);
			for (var i = 0; i < 7; i++)
			{
				Assert.Equal(i, array[i]);
			}
		}

		[Fact]
		public void TestMapJsonToObject43()
		{
			var json = "{\"Name\":\"Ken\", \"Array\":[0, 1, 2, 3, 4, 5, 6]}";
			var obj = JsonMapper.To<IntArray>(json);
			Assert.Equal("Ken", obj.Name);
			Assert.NotNull(obj.Array);
			for (var i = 0; i < 7; i++)
			{
				Assert.Equal(i, obj.Array[i]);
			}
		}

		[Fact]
		public void TestMapJsonToObject44()
		{
			var json = "[[0, 1, 2], [3, 4, 5], [6, 7, 8]]";
			var matrix = JsonMapper.To<int[][]>(json);
			Assert.NotNull(matrix);
			for (var i = 0; i < 3; i++)
			{
				Assert.Equal(i, matrix[0][i]);
				Assert.Equal(i + 3, matrix[1][i]);
				Assert.Equal(i + 6, matrix[2][i]);
			}
		}

		[Fact]
		public void TestMapJsonToObject45()
		{
			var json = "{\"Name\":\"Sean\", \"Children\": [\"Lynn\", \"Daisy\", \"John\"]}";
			var sean = JsonMapper.To<PersonArray>(json);
			Assert.Equal("Sean", sean.Name);
			Assert.NotNull(sean.Children);
			Assert.Equal("Lynn", sean.Children[0]);
			Assert.Equal("Daisy", sean.Children[1]);
			Assert.Equal("John", sean.Children[2]);
		}

		[Fact]
		public void TestMapJsonToObject46()
		{
			var json = "{\"Name\":\"Alan\", \"Matrix\": [[0, 1, 2], [3, 4, 5], [6, 7, 8]]}";
			var alan = JsonMapper.To<MatrixArray>(json);
			Assert.Equal("Alan", alan.Name);
			Assert.NotNull(alan.Matrix);
			for (var i = 0; i < 3; i++)
			{
				Assert.Equal(i, alan.Matrix[0][i]);
				Assert.Equal(i + 3, alan.Matrix[1][i]);
				Assert.Equal(i + 6, alan.Matrix[2][i]);
			}
		}

        [Fact]
        public void TestMapObjectToJson01()
        {
            var simple = new Simple();
            simple.FieldA = "valueA";
            simple.FieldB = 10;
            simple.FieldC = 1.783;
            simple.FieldD = true;
            var json = JsonMapper.ToJson(simple);
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("valueA", (string)jsonObj.FieldA);
            Assert.Equal(10, (int)jsonObj.FieldB);
            Assert.Equal(1.783, (double)jsonObj.FieldC);
            Assert.True((bool)jsonObj.FieldD);
        }

        [Fact]
        public void TestMapObjectToJson02()
        {
            var simple = new Simple
            {
                FieldA = "valueA",
                FieldB = 10,
                FieldC = 1.783,
                FieldD = true
            };
            var nested = new Nested
            {
                FieldE = "valueE",
                FieldF = 20,
                FieldG = 34.0034,
                FieldH = false,
                Simple = simple
            };
            var json = JsonMapper.ToJson(nested);
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("valueA", (string)jsonObj.Simple.FieldA);
            Assert.Equal(10, (int)jsonObj.Simple.FieldB);
            Assert.Equal(1.783, (double)jsonObj.Simple.FieldC);
            Assert.True((bool)jsonObj.Simple.FieldD);
            Assert.Equal("valueE", (string)jsonObj.FieldE);
            Assert.Equal(20, (int)jsonObj.FieldF);
            Assert.Equal(34.0034, (double)jsonObj.FieldG);
            Assert.False((bool)jsonObj.FieldH);
        }

        [Fact]
        public void TestMapObjectToJson03()
        {
            var dict = new Dictionary<string, int>
            {
                {"value1", 1},
                {"value2", 2},
                {"value3", 3}
            };
            var json = JsonMapper.ToJson(dict);
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal(1, (int)jsonObj.value1);
            Assert.Equal(2, (int)jsonObj.value2);
            Assert.Equal(3, (int)jsonObj.value3);
        }

        [Fact]
        public void TestMapObjectToJson04()
        {
            var dict = new Dictionary<int, object>
            {
                {1, new object()},
                {2, new object()},
                {3, new object()}
            };
            var json = JsonMapper.ToJson(dict);
            Assert.True(json.StartsWith("{"));
            Assert.True(json.EndsWith("}"));
            Assert.True(string.IsNullOrWhiteSpace(json.Trim('{').Trim('}')));
        }

        [Fact]
        public void TestMapObjectToJson05()
        {
            var dict = new Dictionary<string, Simple>
            {
                {"Simple1", new Simple {FieldA = "f1", FieldB = 1, FieldC = 10.1, FieldD = true}},
                {"Simple2", new Simple {FieldA = "f2", FieldB = 2, FieldC = 20.5, FieldD = false}},
                {"Simple3", new Simple {FieldA = "f3", FieldB = 3, FieldC = 30.7, FieldD = true }},
                {"Simple4", new Simple {FieldA = "f4", FieldB = 4, FieldC = 40.9, FieldD = false }}
            };
            var json = JsonMapper.ToJson(dict);
            var jsonObj = JsonMapper.Parse(json);
            var s1 = jsonObj.Simple1;
            var s2 = jsonObj.Simple2;
            var s3 = jsonObj.Simple3;
            var s4 = jsonObj.Simple4;
            Assert.Equal("f1", (string)s1.FieldA);
            Assert.Equal("f2", (string)s2.FieldA);
            Assert.Equal("f3", (string)s3.FieldA);
            Assert.Equal("f4", (string)s4.FieldA);

            Assert.Equal(1, (int)s1.FieldB);
            Assert.Equal(2, (int)s2.FieldB);
            Assert.Equal(3, (int)s3.FieldB);
            Assert.Equal(4, (int)s4.FieldB);

            Assert.Equal(10.1, (double)s1.FieldC);
            Assert.Equal(20.5, (double)s2.FieldC);
            Assert.Equal(30.7, (double)s3.FieldC);
            Assert.Equal(40.9, (double)s4.FieldC);

            Assert.True((bool)s1.FieldD);
            Assert.False((bool)s2.FieldD);
            Assert.True((bool)s3.FieldD);
            Assert.False((bool)s4.FieldD);
        }

		[Fact]
		public void TestMapObjectToJson06()
		{
			var json = JsonMapper.ToJson(new {FieldA = "AValue", FieldB = "BValue"});
			dynamic jsonObject = JsonMapper.Parse(json);
			Assert.Equal("AValue", (string)jsonObject.FieldA);
			Assert.Equal("BValue", (string)jsonObject.FieldB);
		}

		[Fact]
		public void TestMapObjectToJson07()
		{
			var json = JsonMapper.ToJson(new[] {1, 2, 3, 4, 5, 6});
			dynamic jsonObject = JsonMapper.Parse(json);
			Assert.Equal(1, (int)jsonObject[0]);
            var list = JsonMapper.To<List<int>>(json);
            Assert.Equal(6, list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                Assert.Equal(i + 1, list[i]);
            }
		}

        [Fact]
        public void TestMapObjectToJson08()
        {
            var json = JsonMapper.ToJson(new List<int>{1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
            dynamic jsonArray = JsonMapper.Parse(json);
            Assert.Equal(10, jsonArray.Length);

            var list = JsonMapper.To<List<int>>(json);
            Assert.Equal(10, list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                Assert.Equal(i + 1, list[i]);
            }
        }

        [Fact]
        public void TestMapObjectToJson09()
        {
            var hashedMap = new HashedMap<string, string>
            {
                {"field1", "value1"},
                {"field2", "value2"},
                {"field3", "value3"},
                {"field4", "value4"}
            };
            var json = JsonMapper.ToJson(hashedMap);
            dynamic jsonObject = JsonMapper.Parse(json);
            Assert.Equal("value1", (string)jsonObject.field1);
            Assert.Equal("value2", (string)jsonObject.field2);
            Assert.Equal("value3", (string)jsonObject.field3);
            Assert.Equal("value4", (string)jsonObject.field4);
        }

        [Fact]
        public void TestMapObjectToJson10()
        {
            var set = new HashedSet<string>
            {
                "field1", "field2", "field3", "field4"
            };
            var json = JsonMapper.ToJson(set);
            var list = JsonMapper.To<List<string>>(json);
            Assert.Equal(4, list.Count);
            Assert.True(list.Contains("field1"));
            Assert.True(list.Contains("field2"));
            Assert.True(list.Contains("field3"));
            Assert.True(list.Contains("field4"));
            Assert.False(list.Contains("field5"));
        }

        [Fact]
        public void TestMapObjectToJson11()
        {
            var hasDate = new HasDate { Birthday = new DateTime(1990, 10, 21), Name = "Jane" };
            var json = JsonMapper.UseDateFormat(string.Empty).ToJson(hasDate);
            var jsonObject = JsonMapper.Parse(json);
            Assert.Equal("Jane", (string)jsonObject.Name);
            var date = DateTime.Parse((string)jsonObject.Birthday);
            Assert.Equal(1990, date.Year);
            Assert.Equal(10, date.Month);
            Assert.Equal(21, date.Day);
        }

		[Fact]
		public void TestMapObjectToJson12()
		{
			var json = JsonMapper.ToJson(new object[] {1, "a string", 6.04, new {FieldA = "ValueA", FieldB = "ValueB"}});
			dynamic jsonObject = JsonMapper.Parse(json);
			Assert.Equal(1, (int)jsonObject[0]);
			Assert.Equal("a string", (string)jsonObject[1]);
			Assert.Equal(6.04, (double)jsonObject[2]);
			Assert.Equal("ValueA", (string)jsonObject[3].FieldA);
			Assert.Equal("ValueB", (string)jsonObject[3].FieldB);
            //TODO: in previous version, no cast is needed for dynamic object.
		}

		[Fact]
		public void TestMapObjectToJson13()
		{
			object obj = null;
			var json = JsonMapper.ToJson(obj);
			Assert.Equal("null", json);
		}

		[Fact]
		public void TestMapObjectToJson14()
		{
			var n = 1000;
			var json = JsonMapper.ToJson(n);
			Assert.Equal("1000", json);
		}

		[Fact]
		public void TestMapObjectToJson15()
		{
			var n = "1000";
			var json = JsonMapper.ToJson(n);
			Assert.Equal("\"1000\"", json);
		}

		[Fact]
		public void TestMapObjectToJson16()
		{
			var n = 1.4435;
			var json = JsonMapper.ToJson(n);
			Assert.Equal("1.4435", json);
		}

		[Fact]
		public void TestMapObjectToJson17()
		{
			var n = 1.2f;
			var json = JsonMapper.ToJson(n);
			Assert.Equal("1.2", json);
		}

		[Fact]
		public void TestMapObjectToJson18()
		{
			var t = true;
			var json = JsonMapper.ToJson(t);
			Assert.Equal("True", json);
		}

		[Fact]
		public void TestMapObjectToJson19()
		{
			var f = false;
			var json = JsonMapper.ToJson(f);
			Assert.Equal("False", json);
		}

		[Fact]
		public void TestMapObjectToJson20()
		{
			uint i = 12012312;
			var json = JsonMapper.ToJson(i);
			Assert.Equal("12012312", json);
		}

		[Fact]
		public void TestMapObjectToJson21()
		{
			var obj = new IListNested {Name = "Joe"};
			var json = JsonMapper.ToJson(obj);
			dynamic nested = JsonMapper.Parse(json);
			Assert.Equal("Joe", (string)nested.Name);
			Assert.Null(nested.Numbers);
		}

        [Fact]
        public void TestMapObjectToJson22()
        {
            var nested = new IListNested { Name = "Joe", Numbers = new List<int> { 1, 2, 3, 4, 5 } };
            var json = JsonMapper.ToJson(nested);
            dynamic jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Joe", (string)jsonObj.Name);
            for (var i = 0; i < 5; i++)
            {
                Assert.Equal(i + 1, (int)jsonObj.Numbers[i]);
            }
        }

        [Fact]
        public void TestMapObjectToJson23()
        {
            var alan = new SimpleStruct { Name = "Alan", Age = 30, Pass = true, ExamDate = DateTime.Parse("2016/3/1"), Score = 90.5 };
            var json = JsonMapper.ToJson(alan);
            dynamic jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Alan", (string)jsonObj.Name);
            Assert.Equal(30, (int)jsonObj.Age);
            Assert.True((bool)jsonObj.Pass);
            Assert.Equal(90.5, (double)jsonObj.Score);
            var date = DateTime.Parse((string)jsonObj.ExamDate);
            Assert.Equal(2016, date.Year);
            Assert.Equal(3, date.Month);
            Assert.Equal(1, date.Day);
        }

		[Fact]
		public void TestMapObjectToJson24()
		{
			var obj = new PrivateSetter { Name = "John" };
			var json = JsonMapper.ToJson(obj);
			dynamic jsonObj = JsonMapper.Parse(json);
			Assert.Equal("John", (string)jsonObj.Name);
			Assert.Equal(23, (int)jsonObj.Age);
		}

		/// <summary>
		/// object has private getter.
		/// </summary>
		[Fact]
		public void TestMapObjectToJson25()
		{
			var obj = new PrivateGetter {Name = "Kevin", Age = 21};
			var json = JsonMapper.ToJson(obj);
			dynamic jsonObj = JsonMapper.Parse(json);
			Assert.Equal("Kevin", (string)jsonObj.Name);
			Assert.False(jsonObj.HasValue("Age"));
		}

		/// <summary>
		/// Dict to json and json to dict.
		/// </summary>
		[Fact]
		public void TestMapObjectToJson27()
		{
			var map = new HashedMap<string, Simple>();
			for (var i = 0; i < 10; i++)
			{
				var simple = new Simple
				{
					FieldA = "value" + i,
					FieldB = i,
					FieldC = i + ((double) i)/10,
					FieldD = i%2 == 0
				};
				map.Add("simple" + i, simple);
			}
			var json = JsonMapper.ToJson(map);
			var dict = JsonMapper.To<Dictionary<string, Simple>>(json);
			for (var i = 0; i < 10; i++)
			{
				var key = "simple" + i;
				var simple = dict[key];
				Assert.Equal("value" + i, simple.FieldA);
				Assert.Equal(i, simple.FieldB);
				Assert.Equal(i + ((double)i)/10, simple.FieldC);
				Assert.Equal(i % 2 == 0, simple.FieldD);
			}

			Assert.Throws(typeof (InvalidOperationException), () => JsonMapper.To<IDictionary<string, Simple>>(json));
		}

		[Fact]
		public void TestMapObjectToJson28()
		{
			var map = new Dictionary<string, Simple>();
			for (var i = 0; i < 10; i++)
			{
				var simple = new Simple
				{
					FieldA = "value" + i,
					FieldB = i,
					FieldC = i + ((double) i)/10,
					FieldD = i%2 == 0
				};
				map.Add("simple" + i, simple);
			}
			var json = JsonMapper.ToJson(map);
			var dict = JsonMapper.To<HashedMap<string, Simple>>(json);
			for (var i = 0; i < 10; i++)
			{
				var key = "simple" + i;
				var simple = dict[key];
				Assert.Equal("value" + i, simple.FieldA);
				Assert.Equal(i, simple.FieldB);
				Assert.Equal(i + ((double)i)/10, simple.FieldC);
				Assert.Equal(i % 2 == 0, simple.FieldD);
			}
		}

		[Fact]
		public void TestMapObjectToJson29()
		{
			var json = JsonMapper.ToJson(Major.Physics);
			Assert.Equal("\"Physics\"", json);
		}

        [Fact]
        public void TestMapObjectToJson30()
        {
            var company = new Company
            {
                Country = "EN",
                Revenue = 1000000,
                StaffCount = 1000
            };

            JsonMapper.For<Company>().MapProperty(x => x.StaffCount).With("Staff Count");

            var json = JsonMapper.ToJson(company);
            var jsonObject = JsonMapper.Parse(json);
            Assert.Null(jsonObject.Employees);
            Assert.Null(jsonObject.Name);

            var newCompany = JsonMapper.To<Company>(json);
            Assert.Null(newCompany.Name);
            Assert.Equal(0, newCompany.Employees.Count);
            Assert.Equal("EN", newCompany.Country);
            Assert.Equal(1000000, newCompany.Revenue);
            Assert.Equal(1000, newCompany.StaffCount);
        }

        [Fact]
        public void TestMapProperty01()
        {
	        JsonMapper.For<ToySet>().MapProperty(x => x.Name).With("ToyName")
									.MapProperty(x => x.ReleaseYear).With("Year")
									.MapProperty(x => x.Price).With("SellPrice")
									.MapProperty(x => x.SetNo).With("SetNumber");
            var toy = new ToySet
            {
                Name = "Lego",
                ReleaseYear = 2015,
                SetNo = 42023,
                Price = 60.5,
                Category = "Technic",
                Producing = false
            };
            var json = JsonMapper.ToJson(toy);
            var jsonObj = JsonMapper.Parse(json);
            Assert.True(jsonObj.HasValue("ToyName"));
            Assert.False(jsonObj.HasValue("Name"));
            Assert.True(jsonObj.HasValue("Year"));
            Assert.False(jsonObj.HasValue("ReleaseYear"));
            Assert.True(jsonObj.HasValue("SellPrice"));
            Assert.False(jsonObj.HasValue("Price"));
            Assert.True(jsonObj.HasValue("SetNumber"));
            Assert.False(jsonObj.HasValue("SetNo"));

            Assert.Equal("Lego", (string)jsonObj.ToyName);
            Assert.Equal("Technic", (string)jsonObj.Category);
            Assert.Equal(2015, (int)jsonObj.Year);
            Assert.Equal(60.5, (double)jsonObj.SellPrice);
            Assert.Equal(42023, (int)jsonObj.SetNumber);
            Assert.False((bool)jsonObj.Producing);
            jsonObj.MadeIn = "Denmark";

            string newJson = jsonObj.ToString();
            var newToy = JsonMapper.To<ToySet>(newJson);
            Assert.Equal("Lego", newToy.Name);
            Assert.Equal(2015, newToy.ReleaseYear);
            Assert.Equal(60.5, newToy.Price);
            Assert.False(newToy.Producing);
            Assert.Equal("Technic", newToy.Category);
            Assert.Equal(42023, newToy.SetNo);
        }

		[Fact]
		public void TestMapProperty02()
		{
			JsonMapper.For<Photo>().MapProperty(x => x.Location).With("Place").Not.MapProperty(x => x.Model);
			var photo = new Photo
			{
				Author = "Owen",
				Location = "Canada",
				Model = "EOS 5D Mark II",
				Time = new DateTime(2011, 5, 20)
			};
			var json = JsonMapper.ToJson(photo);
			var p = JsonMapper.Parse(json);
			Assert.False(p.HasValue("Model"));
			Assert.True(p.HasValue("Place"));
			Assert.False(p.HasValue("Location"));
			Assert.Equal("Owen", (string)p.Author);
			Assert.Equal("Canada", (string)p.Place);
			var shotTime = DateTime.Parse((string) p.Time);
			Assert.Equal(2011, shotTime.Year);
			Assert.Equal(5, shotTime.Month);
			Assert.Equal(20, shotTime.Day);
		}

		[Fact]
		public void TestMapProperty03()
		{
			JsonMapper.UseDateFormat(string.Empty).For<Photo>().MapProperty(x => x.Location).With("Place").Not.MapProperty(x => x.Model);
			var json = "{\"Author\": \"Owen\", \"Place\": \"France\", \"Model\": \"EOS 5D Mark II\", \"Time\": \"2011/05/30\"}";
			var photo = JsonMapper.To<Photo>(json);
			Assert.Equal("Owen", photo.Author);
			Assert.Equal("France", photo.Location);
			Assert.Equal(2011, photo.Time.Year);
			Assert.Equal(5, photo.Time.Month);
			Assert.Equal(30, photo.Time.Day);
			Assert.True(string.IsNullOrWhiteSpace(photo.Model));
		}

        [Fact]
        public void TestMapProperty04()
        {
            JsonMapper.For<Person>().ConstructWith(() => new Person("UK", "Male")).MapProperty(x => x.Name).With("Person Name");
            var json = "{\"Person Name\": \"John\", \"Age\": 24}";
            var person = JsonMapper.To<Person>(json);
            Assert.Equal("John", person.Name);
            Assert.Equal(24, person.Age);
            Assert.Equal("UK", person.Nationality);
            Assert.Equal("Male", person.Gender);

            JsonMapper.For<Person>().ConstructWith(() => new Person("USA", "Female"));
            json = "{\"Person Name\": \"Rose\", \"Age\": 24}";
            person = JsonMapper.To<Person>(json);
            Assert.Equal("Rose", person.Name);
            Assert.Equal(24, person.Age);
            Assert.Equal("USA", person.Nationality);
            Assert.Equal("Female", person.Gender);
        }

		[Fact]
		public void TestMapProperty05()
		{
			JsonMapper.For<Person>().ConstructWith(() => new Person("FR", "Female")).MapProperty(x => x.Name).With("Person Name");
			JsonMapper.For<Student>().MapProperty(x => x.Person).With("Personal Information");
			var json =
				"{\"personal information\": {\"person NAme\": \"Emily\", \"Age\": 21}, \"Major\": \"CS\", \"grade\": 3}";
			var student = JsonMapper.To<Student>(json);
			Assert.Equal("Emily", student.Person.Name);
			Assert.Equal("FR", student.Person.Nationality);
			Assert.Equal("Female", student.Person.Gender);
			Assert.Equal(21, student.Person.Age);
			Assert.Equal(Major.CS, student.Major);
			Assert.Equal(3, student.Grade);
		}

        [Fact]
		public void TestMapProperty06()
		{
			var json = "{\"Birthday\": \"1995-10-05\", \"Name\": \"Joe\"}";
			var hasDate = JsonMapper.UseDateFormat(string.Empty).To<HasDate>(json);
			Assert.Equal("Joe", hasDate.Name);
			Assert.Equal(1995, hasDate.Birthday.Year);
			Assert.Equal(10, hasDate.Birthday.Month);
			Assert.Equal(5, hasDate.Birthday.Day);
		}

		[Fact]
		public void TestMapProperty07()
		{
			var person = new Person("CN", "Male") {Age = 19, Name = "Yang"};
			var student = new Student
			{
				Grade = 2, 
				Major = Major.Art, 
				Person = person, 
				ReportDate = new DateTime(2000, 9, 10)
			};
			JsonMapper.For<Person>().ConstructWith(() => new Person("JP", "Male")).MapProperty(x => x.Name).With("person name");
			JsonMapper.For<Student>().MapProperty(x => x.Person).With("personal information");
			var json = JsonMapper.ToJson(student);
			var jsonObj = JsonMapper.Parse(json);
			Assert.Equal("Yang", (string)jsonObj["personal information"]["person name"]);
			Assert.Equal(19, (int)jsonObj["personal information"]["Age"]);
			Assert.Equal("CN", (string)jsonObj["personal information"]["Nationality"]);
			Assert.Equal("Male", (string)jsonObj["personal information"]["Gender"]);
			Assert.Equal("Art", (string)jsonObj["Major"]);
			Assert.Equal(2, (int)jsonObj["Grade"]);
			var dt = DateTime.Parse((string) jsonObj["ReportDate"]);
			Assert.Equal(2000, dt.Year);
			Assert.Equal(9, dt.Month);
			Assert.Equal(10, dt.Day);
			var newStudent = JsonMapper.To<Student>(json);
			Assert.Equal("Yang", newStudent.Person.Name);
			Assert.Equal(19, newStudent.Person.Age);
			Assert.Equal("JP", newStudent.Person.Nationality);
			Assert.Equal("Male", newStudent.Person.Gender);
			Assert.Equal(Major.Art, newStudent.Major);
			Assert.Equal(2, newStudent.Grade);
			Assert.Equal(2000, newStudent.ReportDate.Year);
			Assert.Equal(9, newStudent.ReportDate.Month);
			Assert.Equal(10, newStudent.ReportDate.Day);
		}

        [Fact]
		public void TestMapProperty08()
		{
			var json = "{\"Birthday\": \"10/05/1995\", \"Name\": \"Joe\"}";
			var hasDate = JsonMapper.UseDateFormat("MM/dd/yyyy").To<HasDate>(json);
			Assert.Equal("Joe", hasDate.Name);
			Assert.Equal(1995, hasDate.Birthday.Year);
			Assert.Equal(10, hasDate.Birthday.Month);
			Assert.Equal(5, hasDate.Birthday.Day);
		}

		[Fact]
		public void TestMapProperty09()
		{
			JsonMapper.For<Person>()
				.ConstructWith(() => new Person("US", "Male"))
				.MapProperty(x => x.Name).With("Person Name");
			var json =
				"[{\"person name\": \"Jackson\", \"Age\": 22}, {\"person name\": \"Johnson\", \"Age\": 21}, {\"person name\": \"Hugo\", \"Age\": 23}]";
			var people = JsonMapper.To<List<Person>>(json);
			Assert.Equal(3, people.Count);
			Assert.Equal("Jackson", people[0].Name);
			Assert.Equal(22, people[0].Age);
			Assert.Equal("US", people[0].Nationality);
			Assert.Equal("Male", people[0].Gender);
			Assert.Equal("Johnson", people[1].Name);
			Assert.Equal(21, people[1].Age);
			Assert.Equal("US", people[1].Nationality);
			Assert.Equal("Male", people[1].Gender);
			Assert.Equal("Hugo", people[2].Name);
			Assert.Equal(23, people[2].Age);
			Assert.Equal("US", people[2].Nationality);
			Assert.Equal("Male", people[2].Gender);

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
			Assert.Equal(20, (parseEngine.Parse(json) as JInteger).AsInt32());
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
			Assert.Equal(27, (jack["age"] as JInteger).AsInt32());
			Assert.Equal("usa", jack["country"] as JString);
			var jon = (value[1] as JObject)["Jon"] as JObject;
			Assert.NotNull(jon);
			Assert.Equal(21, (jon["age"] as JInteger).AsInt32());
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
