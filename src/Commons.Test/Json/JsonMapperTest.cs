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
using Commons.Collections.Map;
using Commons.Collections.Set;
using Commons.Json;
using Commons.Json.Mapper;
using Xunit;
using System.Text;
using System.IO;
using System.Collections;
using Commons.Test.Poco;

namespace Commons.Test.Json
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

		[Fact]
		public void TestMapJsonToObject011()
		{
			var obj = JsonMapper.To<Simple>("{'FieldA': 'valueA', 'FieldB' : 10, 'FieldC': 2.3, 'FieldD': true}");
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
			var json = "{\"FieldE\": \"valueE\", \"FieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
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
			var json = "{\"FieldE\": \"valueE\", \"FieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
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
			var json = "{\"FieldE\": null, \"FieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
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
			var json = "{\"FieldE\": null, \"FieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
					   + "\"valueA\", \"FieldB\": 0, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
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
			var json = "{\"Fielde\": 33, \"FieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
					   + "\"valueA\", \"FieldB\": null, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<Nested>(json));
		}

		[Fact]
		public void TestMapJsonToObject07()
		{
			var json = "{\"Fielde\": \"valueE\", \"FieldF\": 20, \"FieldG\": 3, \"Simple\": {\"FieldA\": "
					   + "\"valueA\", \"FieldB\": 0, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
			var nested = JsonMapper.To<Nested>(json);
			Assert.Equal(3, nested.FieldG, 1);
		}

		[Fact]
		public void TestMapJsonToObject08()
		{
			var json = "{\"FieldF\": 20, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
					   + "\"valueA\", \"FieldB\": 0, \"FieldC\": 1.2997, \"FieldD\": false}, \"FieldH\": true, \"FieldI\": \"valueI\"}";
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
			var json = "{\"FieldF\": 20.543, \"FieldG\": 3.459, \"Simple\": {\"FieldA\": "
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
			var json = "{\"FieldJ\": \"valueJ\", \"FieldK\": [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20]}";
			var primitiveList = JsonMapper.To<PrimitiveList>(json);
			Assert.Equal("valueJ", primitiveList.FieldJ);
			for (var i = 0; i < primitiveList.FieldK.Count; i++)
			{
				Assert.Equal(i + 10, primitiveList.FieldK[i]);
			}
		}

		[Fact]
		public void TestMapJsonToObject121()
		{
			var json = "{\"FieldJ\": \"valueJ\", \"FieldK\": null}";
			var primitiveList = JsonMapper.To<PrimitiveList>(json);
			Assert.Equal("valueJ", primitiveList.FieldJ);
			Assert.Null(primitiveList.FieldK);
		}

		[Fact]
		public void TestMapJsonToObject122()
		{
			var json = "{\"FieldJ\": \"valueJ\", \"FieldK\": []}";
			var primitiveList = JsonMapper.To<PrimitiveList>(json);
			Assert.Equal("valueJ", primitiveList.FieldJ);
			Assert.NotNull(primitiveList.FieldK);
			Assert.Equal(0, primitiveList.FieldK.Count);
		}

		[Fact]
		public void TestMapJsonToObject13()
		{
			var json = TestHelper.ReadFrom("JsonSample11.txt");
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
		public void TestMapJsonToObject141()
		{
			var guid = Guid.NewGuid();
			var json = "\"" + guid.ToString() + "\"";
			var newGuid = JsonMapper.To<Guid>(json);
			Assert.Equal(guid, newGuid);
		}

		[Fact]
		public void TestMapJsonToObject142()
		{
			var guid = Guid.NewGuid();
			var json = "\"" + guid.ToString() + "\"";
			var newGuid = JsonMapper.To<Guid?>(json);
			Assert.Equal(guid, newGuid.Value);
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
			var json = "{\"FieldB\": 10}";
			var simple = JsonMapper.To<Simple>(json);
			Assert.Equal(10, simple.FieldB);
			Assert.Null(simple.FieldA);
			Assert.Equal(0, simple.FieldC);
			Assert.False(simple.FieldD);
		}

		[Fact]
		public void TestMapJsonToObject28()
		{
			var json = "{\"FieldB\": 10, \"NotExistInObject\": \"aaaa\"}";
			var simple = JsonMapper.To<Simple>(json);
			Assert.Equal(10, simple.FieldB);
			Assert.Null(simple.FieldA);
			Assert.Equal(0, simple.FieldC);
			Assert.False(simple.FieldD);
		}

		[Fact]
		public void TestMapJsonToObject29()
		{
			var json = "{\"FieldL\":\"valuel\", \"FieldM\": [1, 2, 3, 4 ,5 ,6]}";
			var setNested = JsonMapper.To<SetNested>(json);
			Assert.Equal("valuel", setNested.FieldL);
			Assert.NotNull(setNested.FieldM);
			Assert.Equal(6, setNested.FieldM.Count);
			for (var i = 0; i < 6; i++)
			{
				Assert.True(setNested.FieldM.Contains(i + 1));
			}
		}

		[Fact]
		public void TestMapJsonToObject291()
		{
			var json = "{\"FieldL\":\"valuel\", \"FieldM\": null}";
			var setNested = JsonMapper.To<SetNested>(json);
			Assert.Equal("valuel", setNested.FieldL);
			Assert.Null(setNested.FieldM);
		}

		[Fact]
		public void TestMapJsonToObject292()
		{
			var json = "{\"FieldL\":\"valuel\", \"FieldM\": []}";
			var setNested = JsonMapper.To<SetNested>(json);
			Assert.Equal("valuel", setNested.FieldL);
			Assert.NotNull(setNested.FieldM);
			Assert.Equal(0, setNested.FieldM.Count);
		}

		[Fact]
		public void TestMapJsonToObject30()
		{
			var json = "{\"FieldL\": 10, \"FieldM\": [1, 2, 3, 4 ,5 ,6]}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<SetNested>(json));
		}

		[Fact]
		public void TestMapJsonToObject31()
		{
			var json = "{\"FieldL\": {}, \"FieldM\": [1, 2, 3, 4 ,5 ,6]}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<SetNested>(json));
		}

		[Fact]
		public void TestMapJsonToObject32()
		{
			var json = "{\"Birthday\": \"1990/01/18\", \"Name\": \"alan\"}";
			var hasDate = JsonMapper.NewContext().UseDateFormat("yyyy/MM/dd").To<HasDate>(json);
			Assert.Equal("alan", hasDate.Name);
			Assert.Equal(1990, hasDate.Birthday.Year);
			Assert.Equal(1, hasDate.Birthday.Month);
			Assert.Equal(18, hasDate.Birthday.Day);
		}

		[Fact]
		public void TestMapJsonToObject33()
		{
			var json = "{\"Birthday\": \"1990/1/5/88\", \"Name\": \"alan\"}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.NewContext().UseDateFormat(string.Empty).To<HasDate>(json));
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
			var simple = JsonMapper.NewContext().UseDateFormat("yyyy/MM/dd").To<SimpleStruct>(json);
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
			var json = "{\"FieldJ\": \"valuej\", \"FieldK\": [1, 2, 4a5, 6]}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<PrimitiveList>(json));
		}

		[Fact]
		public void TestMapJsonToObject401()
		{
			var json = "{\"FieldJ\": \"valuej\", \"FieldK\": [1, 2, a5a, 6]}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<PrimitiveList>(json));
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
		public void TestMapJsonToObject431()
		{
			var json = "{\"Name\":\"Ken\", \"Array\":[]}";
			var obj = JsonMapper.To<IntArray>(json);
			Assert.Equal("Ken", obj.Name);
			Assert.NotNull(obj.Array);
			Assert.Equal(0, obj.Array.Length);
		}

		[Fact]
		public void TestMapJsonToObject432()
		{
			var json = "{\"Name\":\"Ken\", \"Array\":null}";
			var obj = JsonMapper.To<IntArray>(json);
			Assert.Equal("Ken", obj.Name);
			Assert.Null(obj.Array);
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
		public void TestMapJsonToObject47()
		{
			var json = "{\"companies\": {\"abc\": \"abc company\", \"def\":\"def company\"}}";
			var dict = JsonMapper.To<Dictionary<string, Dictionary<string, string>>>(json);
			Assert.Equal(1, dict.Count);
			Assert.Equal(2, dict["companies"].Count);
			Assert.Equal("abc company", dict["companies"]["abc"]);
			Assert.Equal("def company", dict["companies"]["def"]);
		}

		[Fact]
		public void TestMapJsonToObject48()
		{
			var json =
				"{\"Name\": \"A History of Europe\", \"Author\": \"Edward\", \"Pages\": 2000, \"PublishDate\": \"1750/7/5\"}";
			var artical = JsonMapper.NewContext().UseDateFormat("yyyy/M/d").To<Artical>(json);
			Assert.Equal("A History of Europe", artical.Name);
			Assert.Equal("Edward", artical.Author);
			Assert.Equal(2000, artical.Pages);
			Assert.Equal(1750, artical.PublishDate.Value.Year);
			Assert.Equal(7, artical.PublishDate.Value.Month);
			Assert.Equal(5, artical.PublishDate.Value.Day);
		}

		[Fact]
		public void TestMapJsonToObject49()
		{
			var json = "5";
			var number1 = JsonMapper.To<int?>(json);
			Assert.Equal(5, number1.Value);

			var number2 = JsonMapper.To<short?>(json);
			Assert.Equal(5, number2.Value);

			var number3 = JsonMapper.To<byte?>(json);
			Assert.Equal(5, number3.Value);

			var number4 = JsonMapper.To<sbyte?>(json);
			Assert.Equal(5, number4.Value);

			var number5 = JsonMapper.To<uint?>(json);
			Assert.Equal((uint)5, number5.Value);

			var number6 = JsonMapper.To<long?>(json);
			Assert.Equal(5, number6.Value);

			var number7 = JsonMapper.To<ulong?>(json);
			Assert.Equal((ulong)5, number7.Value);

			var number8 = JsonMapper.To<ushort?>(json);
			Assert.Equal(5, number8.Value);

            var number9 = JsonMapper.To<double?>(json);
            Assert.Equal((double)5, number9.Value);

            var number10 = JsonMapper.To<decimal?>(json);
            Assert.Equal((decimal)5, number10.Value);

            var number11 = JsonMapper.To<float?>(json);
            Assert.Equal((float)5, number11.Value);
		}

        [Fact]
        public void TestMapJsonToObject491()
        {
            var json = "\"a\"";
            var ch = JsonMapper.To<char>(json);
            Assert.Equal('a', ch);

            var ch2 = JsonMapper.To<char?>(json);
            Assert.Equal('a', ch2.Value);
        }

        [Fact]
        public void TestMapJsonToObject492()
        {
            var json = "\"05/06/2016 13:20:50.324\"";
            var dt = JsonMapper.To<DateTime>(json);
            Assert.Equal(2016, dt.Year);
            Assert.Equal(5, dt.Month);
            Assert.Equal(6, dt.Day);
            Assert.Equal(13, dt.Hour);
            Assert.Equal(20, dt.Minute);
            Assert.Equal(50, dt.Second);
            Assert.Equal(324, dt.Millisecond);
        }

        [Fact]
        public void TestMapJsonToObject493()
        {
            var json = "\"05/06/2016 13:20:50.324\"";
            var dt = JsonMapper.To<DateTime?>(json);
            Assert.Equal(2016, dt.Value.Year);
            Assert.Equal(5, dt.Value.Month);
            Assert.Equal(6, dt.Value.Day);
            Assert.Equal(13, dt.Value.Hour);
            Assert.Equal(20, dt.Value.Minute);
            Assert.Equal(50, dt.Value.Second);
            Assert.Equal(324, dt.Value.Millisecond);
        }

        [Fact]
        public void TestMapjsonToObject494()
        {
            var json = "null";
            Assert.Null(JsonMapper.To<int?>(json));
            Assert.Null(JsonMapper.To<uint?>(json));
            Assert.Null(JsonMapper.To<short?>(json));
            Assert.Null(JsonMapper.To<ushort?>(json));
            Assert.Null(JsonMapper.To<long?>(json));
            Assert.Null(JsonMapper.To<ulong?>(json));
            Assert.Null(JsonMapper.To<char?>(json));
            Assert.Null(JsonMapper.To<byte?>(json));
            Assert.Null(JsonMapper.To<sbyte?>(json));
            Assert.Null(JsonMapper.To<double?>(json));
            Assert.Null(JsonMapper.To<float?>(json));
            Assert.Null(JsonMapper.To<decimal?>(json));
            Assert.Null(JsonMapper.To<Site?>(json));
            Assert.Null(JsonMapper.To<SimpleStruct?>(json));
            Assert.Null(JsonMapper.To<int[]>(json));
            Assert.Null(JsonMapper.To<Dictionary<string, string>>(json));
            Assert.Null(JsonMapper.To<bool?>(json));

            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<int>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<uint>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<long>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<ulong>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<short>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<ushort>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<byte>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<sbyte>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<char>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<Site>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<SimpleStruct>(json));
            Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<bool>(json));
        }

		[Fact]
		public void TestMapJsonToObject50()
		{
			var json = "true";
			var v = JsonMapper.To<bool?>(json);
			Assert.True(v.Value);
		}

		[Fact]
		public void TestMapJsonToObject51()
		{
			var json = "\"SH\"";
			var site = JsonMapper.To<Site?>(json);
			Assert.Equal(Site.SH, site.Value);
		}

		public void TestMapJsonToObject52()
		{
			var json =
				"{\"Name\":\"Sam\", \"Age\":50,\"Score\":80.5, \"ExamDate\":\"2016/4/15\", \"Pass\":false}";
			var simple = JsonMapper.NewContext().UseDateFormat(string.Empty).To<SimpleStruct?>(json);
			Assert.NotNull(simple);
			Assert.Equal(50, simple.Value.Age);
			Assert.Equal("Sam", simple.Value.Name);
			Assert.Equal(80.5, simple.Value.Score);
			Assert.Equal(2016, simple.Value.ExamDate.Year);
			Assert.Equal(4, simple.Value.ExamDate.Month);
			Assert.Equal(15, simple.Value.ExamDate.Day);
			Assert.False(simple.Value.Pass);
		}

		[Fact]
		public void TestMapJsonToObject53()
		{
			var json =
				"{\"Name\": \"Complex\", \"Simple\": {\"Name\":\"Sam\", \"Age\":50,\"Score\":80.5, \"ExamDate\":\"2016/4/15\", \"Pass\":false}}";
			var complicated = JsonMapper.NewContext().UseDateFormat("yyyy/M/dd").To<Complicated>(json);
			Assert.Equal("Complex", complicated.Name);
			Assert.NotNull(complicated.Simple);
			Assert.Equal("Sam", complicated.Simple.Value.Name);
			Assert.Equal(50, complicated.Simple.Value.Age);
			Assert.Equal(80.5, complicated.Simple.Value.Score);
			Assert.Equal(2016, complicated.Simple.Value.ExamDate.Year);
			Assert.Equal(4, complicated.Simple.Value.ExamDate.Month);
			Assert.Equal(15, complicated.Simple.Value.ExamDate.Day);
			Assert.False(complicated.Simple.Value.Pass);
		}

		[Fact]
		public void TestMapJsonToObject54()
		{
			var json =
				"{\"Name\": \"Complex\", \"Simple\": {\"Name\":\"Sam\", \"Age\":50,\"Score\":80.5, \"ExamDate\":\"2016/4/15\", \"Pass\":false}}";
			var complicated = JsonMapper.NewContext().UseDateFormat("yyyy/M/dd").To<Complicated?>(json);
			Assert.NotNull(complicated);
			Assert.Equal("Complex", complicated.Value.Name);
			Assert.NotNull(complicated.Value.Simple);
			Assert.Equal("Sam", complicated.Value.Simple.Value.Name);
			Assert.Equal(50, complicated.Value.Simple.Value.Age);
			Assert.Equal(80.5, complicated.Value.Simple.Value.Score);
			Assert.Equal(2016, complicated.Value.Simple.Value.ExamDate.Year);
			Assert.Equal(4, complicated.Value.Simple.Value.ExamDate.Month);
			Assert.Equal(15, complicated.Value.Simple.Value.ExamDate.Day);
			Assert.False(complicated.Value.Simple.Value.Pass);
		}

		[Fact]
		public void TestMapJsonToObject55()
		{
			var json = "{\"Name\": \"Complex\"}";
			var complicated = JsonMapper.To<Complicated>(json);
			Assert.Equal("Complex", complicated.Name);
			Assert.Null(complicated.Simple);
		}

		[Fact]
		public void TestMapJsonToObject56()
		{
			var json = "{\"Name\": \"Complex\"}";
			var complicated = JsonMapper.To<Complicated?>(json);
			Assert.Equal("Complex", complicated.Value.Name);
			Assert.Null(complicated.Value.Simple);
		}

		[Fact]
		public void TestMapJsonToObject57()
		{
			var json = "{}";
			var simple = JsonMapper.To<Simple>(json);
			Assert.NotNull(simple);
			Assert.Null(simple.FieldA);
		}

		[Fact]
		public void TestMapJsonToObject58()
		{
			var json = "[{}]";
			var simples = JsonMapper.To<Simple[]>(json);
			Assert.NotNull(simples);
			Assert.Equal(1, simples.Length);
			Assert.NotNull(simples[0]);
			Assert.Null(simples[0].FieldA);
		}

		[Fact]
		public void TestMapJsonToObject59()
		{
			var json = "{\"Name\": \"Lucy\", \"Children\": \"Gavin\"}";
			var person = JsonMapper.To<PersonArray>(json);
			Assert.Equal("Lucy", person.Name);
			Assert.Equal(1, person.Children.Length);
			Assert.Equal("Gavin", person.Children[0]);
		}

		[Fact]
		public void TestMapJsonToObject60()
		{
			var json = "{\"Name\": \"Lucy\", \"Children\": [\"Gavin\", \"Tom\"]}";
			var person = JsonMapper.To<PersonArray>(json);
			Assert.Equal("Lucy", person.Name);
			Assert.Equal(2, person.Children.Length);
			Assert.Equal("Gavin", person.Children[0]);
			Assert.Equal("Tom", person.Children[1]);
		}

		[Fact]
		public void TestMapJsonToObject61()
		{
			var json = "{\"Name\": \"Sam\", \"Array\": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]}";
			var obj = JsonMapper.To<LinkedListNested>(json);
			Assert.Equal("Sam", obj.Name);
			Assert.Equal(11, obj.Array.Count);
			for (var i = 0; i < 11; i++)
			{
				Assert.True(obj.Array.Contains(i));
			}
		}

		[Fact]
		public void TestMapJsonToObject62()
		{
			var json = "{\"Name\": \"Sam\", \"Array\": []}";
			var obj = JsonMapper.To<LinkedListNested>(json);
			Assert.Equal("Sam", obj.Name);
			Assert.Equal(0, obj.Array.Count);
		}

		[Fact]
		public void TestMapJsonToObject63()
		{
			var json = "{\"Name\": \"Sam\", \"Array\": null}";
			var obj = JsonMapper.To<LinkedListNested>(json);
			Assert.Equal("Sam", obj.Name);
			Assert.Null(obj.Array);
		}

		[Fact]
		public void TestMapJsonToObject64()
		{
			var json = "[0, 1, 2, 3, 4, 5, 6, 7]";
			var queue = JsonMapper.To<Queue<int>>(json);
			Assert.Equal(8, queue.Count);
			var i = 0;
			while (queue.Count > 0)
			{
				var value = queue.Dequeue();
				Assert.Equal(i, value);
				i++;
			}
		}

		[Fact]
		public void TestMapJsonToObject65()
		{
			var json = "null";
			var queue = JsonMapper.To<Queue<int>>(json);
			Assert.Null(queue);

			var list = JsonMapper.To<List<int>>(json);
			Assert.Null(list);

			var ll = JsonMapper.To<LinkedList<int>>(json);
			Assert.Null(ll);
		}

		[Fact]
		public void TestMapJsonToObject66()
		{
			var json = "[]";
			var list = JsonMapper.To<List<int>>(json);
			Assert.NotNull(list);
			Assert.Equal(0, list.Count);

			var queue = JsonMapper.To<Queue<int>>(json);
			Assert.NotNull(queue);
			Assert.Equal(0, queue.Count);

			var ll = JsonMapper.To<LinkedList<int>>(json);
			Assert.NotNull(ll);
			Assert.Equal(0, ll.Count);
		}

		[Fact]
		public void TestMapJsonToObject67()
		{
			var json = "[0, 1, 2, 3, 4, 5, 6, 7, 8]";
			var stack = JsonMapper.To<Stack<int>>(json);
			Assert.NotNull(stack);
			Assert.Equal(9, stack.Count);
			var i = 8;
			while (stack.Count > 0)
			{
				Assert.Equal(i, stack.Pop());
				i--;
			}
		}

		[Fact]
		public void TestMapJsonToObject68()
		{
			var json = "null";
			var str = JsonMapper.To<string>(json);
			Assert.Equal(null, str);
			var list = JsonMapper.To<List<int>>(json);
			Assert.Null(list);

			var array = JsonMapper.To<int[]>(json);
			Assert.Null(array);

			var obj = JsonMapper.To<Simple>(json);
			Assert.Null(obj);

			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<SimpleStruct>(json));

			Assert.Null(JsonMapper.To<SimpleStruct?>(json));

			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<int>(json));
		}

		[Fact]
		public void TestMapJsonToObject69()
		{
			var json = "{\"a\":\"b\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<string>(json));
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<int>(json));
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<int[]>(json));
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<List<int>>(json));
		}

		[Fact]
		public void TestMapJsonToObject70()
		{
			var json = "[{\"a\":\"b\", \"g\":\"h\"}, {\"c\":\"d\"}, {\"e\":\"f\"}]";
			var dictArray = JsonMapper.To<Dictionary<string, string>[]>(json);
			Assert.True(dictArray[0].ContainsKey("a"));
			Assert.Equal("b", dictArray[0]["a"]);

			Assert.True(dictArray[0].ContainsKey("g"));
			Assert.Equal("h", dictArray[0]["g"]);

			Assert.True(dictArray[1].ContainsKey("c"));
			Assert.Equal("d", dictArray[1]["c"]);

			Assert.True(dictArray[2].ContainsKey("e"));
			Assert.Equal("f", dictArray[2]["e"]);
		}

		[Fact]
		public void TestMapJsonToObject71()
		{
			var json = "{\"Name\": \"Ben\", \"Excellent\": {\"Reason\": 5}}";
            var won = JsonMapper.NewContext().For<IExcellent>(x => x.ConstructWith(() => new Excellent())).To<Wonderful>(json);
			Assert.Equal("Ben", won.Name);
			Assert.NotNull(won.Excellent);
			Assert.Equal(5, won.Excellent.Reason);
		}

		[Fact]
		public void TestMapJsonToObject72()
		{
			var json = "{\"Name\": \"Alex\", \"Numbers\": [0.1, 0.9, 0.1234, 0.569, 0.89]}";
            var obj = JsonMapper.NewContext().For<IEnumerable<double>>(x => x.ConstructWith(() => new List<double>())).To<HasEnumerable>(json);
			Assert.Equal("Alex", obj.Name);
			Assert.NotNull(obj.Numbers);
			var index = 0;
			foreach (var i in obj.Numbers)
			{
				if (index == 0)
				{
					Assert.Equal(0.1, i);
				}
				else if (index == 1)
				{
					Assert.Equal(0.9, i);
				}
				else if (index == 2)
				{
					Assert.Equal(0.1234, i);
				}
				else if (index == 3)
				{
					Assert.Equal(0.569, i);
				}
				else if (index == 4)
				{
					Assert.Equal(0.89, i);
				}
				index++;
			}
		}

		[Fact]
		public void TestMapJsonToObject73()
		{
			var json = "{\"Name\": \"Ben\", \"SomeAwesomeThing\": {\"TheReason\":\"because it's very nice\"}}";
			var awe = JsonMapper.NewContext().For<IAwesome>(x => 
                                                                x.ConstructWith(y => 
                                                                {
                                                                    var jsonObj = y as JObject;
                                                                    var reason = jsonObj.GetString("TheReason");
                                                                    var a = new Awesome();
                                                                    a.Reason = reason;
                                                                    return a;
                                                                }
                                                            )).To<HasInterface>(json);

			Assert.Equal("Ben", awe.Name);
			Assert.NotNull(awe.SomeAwesomeThing);
			Assert.Equal("because it's very nice", awe.SomeAwesomeThing.Reason);
		}

		[Fact]
		public void TestMapJsonToObject731()
		{
			var json = "[{\"AweReason\":\"nice\"}, {\"AweReason\":\"beautiful\"}, {\"AweReason\":\"elegent\"}]";
            var context = JsonMapper.NewContext().For<IAwesome>(y => y.ConstructWith(x =>
            {
                var jsonObj = x as JObject;
                var reason = jsonObj.GetString("AweReason");
                var a = new Awesome();
                a.Reason = reason;
                return a;
            }));

            var aweArray = context.To<IAwesome[]>(json);

			Assert.Equal(3, aweArray.Length);
			Assert.Equal("nice", aweArray[0].Reason);
			Assert.Equal("beautiful", aweArray[1].Reason);
			Assert.Equal("elegent", aweArray[2].Reason);

			var aweList = context.To<List<IAwesome>>(json);
			Assert.Equal(3, aweList.Count);
			Assert.Equal("nice", aweList[0].Reason);
			Assert.Equal("beautiful", aweList[1].Reason);
			Assert.Equal("elegent", aweList[2].Reason);
		}

		public void TestMapJsonToObject732()
		{
			var json = "[{\"Reason\": 1 }, {\"Reason\": 2 }, {\"Reason\": 3}]";
            var aweArray = JsonMapper.NewContext().For<IExcellent>(y => y.ConstructWith(x => new Excellent())).To<IExcellent[]>(json);

			Assert.Equal(3, aweArray.Length);
			Assert.Equal(1, aweArray[0].Reason);
			Assert.Equal(2, aweArray[1].Reason);
			Assert.Equal(3, aweArray[2].Reason);

			var aweList = JsonMapper.To<List<IExcellent>>(json);
			Assert.Equal(3, aweList.Count);
			Assert.Equal(1, aweList[0].Reason);
			Assert.Equal(2, aweList[1].Reason);
			Assert.Equal(3, aweList[2].Reason);

		}

		[Fact]
		public void TestMapJsonToObject74()
		{
			var json = "{\"FieldI\": \"valuei\", \"NestedItems\": [null, null, null]}";
			var arrayInside = JsonMapper.To<ArrayNested>(json);
			Assert.Equal("valuei", arrayInside.FieldI);
			Assert.NotNull(arrayInside.NestedItems);
			Assert.Equal(3, arrayInside.NestedItems.Count);
			Assert.Null(arrayInside.NestedItems[0]);
			Assert.Null(arrayInside.NestedItems[1]);
			Assert.Null(arrayInside.NestedItems[2]);
		}

		[Fact]
		public void TestMapJsonToObject75()
		{
			var json = "{\"FieldJ\": \"a broken\nline\", \"FieldK\": [0, 1]}";
			var obj1 = JsonMapper.To<PrimitiveList>(json);
			var str = obj1.FieldJ;
			using (var sr = new StringReader(str))
			{
				var line1 = sr.ReadLine();
				Assert.Equal("a broken", line1);
				var line2 = sr.ReadLine();
				Assert.Equal("line", line2);
			}

			var json2 = "{\"FieldJ\": \"d:\\test\\\", \"FieldK\": [0, 1]}";
			var obj2 = JsonMapper.To<PrimitiveList>(json2);
			Assert.Equal("d:\\test\\", obj2.FieldJ);

			var json3 = "{\"FieldJ\": \"~/test/workspace\", \"FieldK\": [0, 1]}";
			var obj3 = JsonMapper.To<PrimitiveList>(json3);
			Assert.Equal("~/test/workspace", obj3.FieldJ);
		}

		[Fact]
		public void TestMapJsonToObject76()
		{
			var json = "{}";
			var dict = JsonMapper.To<Dictionary<string, string>>(json);
			Assert.NotNull(dict);
			Assert.Equal(0, dict.Count);
		}

		[Fact]
		public void TestMapJsonToObject77()
		{
			var json = "[]";
			var array = JsonMapper.To<int[]>(json);
			Assert.NotNull(array);
			Assert.Equal(0, array.Length);
		}

		[Fact]
		public void TestMapJsonToObject78()
		{
			var json = "[{}]";
			var array = JsonMapper.To<List<Dictionary<string, string>>>(json);
			Assert.NotNull(array);
			Assert.Equal(1, array.Count);
			Assert.NotNull(array[0]);
			Assert.Equal(0, array[0].Count);
		}

		[Fact]
		public void TestMapJsonToObject79()
		{
			var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
			var bytes = new byte[20];
			rand.NextBytes(bytes);
			var str = Convert.ToBase64String(bytes);
			var array = new ByteArray
			{
				Bytes = bytes,
				Name = str
			};

			var json = JsonMapper.ToJson(array);
			var result = JsonMapper.To<ByteArray>(json);
			for (var i = 0; i < bytes.Length; i++)
			{
				Assert.Equal(result.Bytes[i], bytes[i]);
			}
			Assert.Equal(str, result.Name);
		}

		[Fact]
		public void TestMapJsonToObject791()
		{
			var json = "{\"Bytes\": [0, 1, 2 ,3 ,4, 5, 6], \"Name\": \"AnArray\"}";
			var result = JsonMapper.To<ByteArray>(json);
			for (var i = 0; i < 7; i++)
			{
				Assert.Equal(i, result.Bytes[i]);
			}
			Assert.Equal("AnArray", result.Name);
		}

		[Fact]
		public void TestMapJsonToObject80()
		{
			var json = "{\"ItemA\": \"123\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<Dictionary<string, int>>(json));

			var dict2 = JsonMapper.To<Dictionary<string, string>>(json);
			Assert.Equal("123", dict2["ItemA"]);

		}

		[Fact]
		public void TestMapJsonToObject801()
		{
			var json = "{\"ItemA\": \"AAA\"}";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<Dictionary<string, int>>(json));
		}

		[Fact]
		public void TestMapJsonToObject81()
		{
			var json = "2.5";
			Assert.Equal(2.5, JsonMapper.To<double>(json));
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<int>(json));

			var another = "2";
			Assert.Equal(2, JsonMapper.To<double>(another));
		}

		[Fact]
		public void TestMapJsonToObject82()
		{
			var time = DateTime.Now;
			var json = JsonMapper.NewContext().UseDateFormat(string.Empty).ToJson(time);
			var result = JsonMapper.To<DateTime>(json);
			Assert.Equal(time.Year, result.Year);
			Assert.Equal(time.Month, result.Month);
			Assert.Equal(time.Day, result.Day);
			Assert.Equal(time.Hour, result.Hour);
			Assert.Equal(time.Minute, result.Minute);
			Assert.Equal(time.Second, result.Second);
			Assert.Equal(time.Millisecond, result.Millisecond);
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
            Assert.Throws(typeof(InvalidOperationException), () => JsonMapper.ToJson(dict));
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
            var json = JsonMapper.NewContext().UseDateFormat(string.Empty).ToJson(hasDate);
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
            Assert.Equal("true", json);
        }

        [Fact]
        public void TestMapObjectToJson19()
        {
            var f = false;
            var json = JsonMapper.ToJson(f);
            Assert.Equal("false", json);
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

            Assert.Null(JsonMapper.To<IDictionary<string, Simple>>(json));
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

            var context = JsonMapper.NewContext().For<Company>(y => y.MapProperty(x => x.StaffCount).With("Staff Count"));

            var json = context.ToJson(company);
            var jsonObject = JsonMapper.Parse(json);
            Assert.Null(jsonObject.Employees);
            Assert.Null(jsonObject.Name);

            var newCompany = context.To<Company>(json);
            Assert.Null(newCompany.Name);
            Assert.Null(newCompany.Employees);
            Assert.Equal("EN", newCompany.Country);
            Assert.Equal(1000000, newCompany.Revenue);
            Assert.Equal(1000, newCompany.StaffCount);
        }

        [Fact]
        public void TestMapObjectToJson31()
        {
            var artical = new Artical
            {
                Author = "Edward",
                Name = "A History of Europe",
                Pages = 2000,
                PublishDate = new DateTime(1985, 7, 5)
            };

            var json = JsonMapper.ToJson(artical);
            var atc = JsonMapper.Parse(json);
            Assert.Equal("Edward", (string)atc.Author);
            Assert.Equal("A History of Europe", (string)atc.Name);
            Assert.Equal(2000, (int)atc.Pages);
            var dt = DateTime.Parse((string) atc.PublishDate);
            Assert.Equal(1985, dt.Year);
            Assert.Equal(7, dt.Month);
            Assert.Equal(5, dt.Day);
        }

        [Fact]
        public void TestMapObjectToJson32()
        {
            Complicated? complicated = new Complicated
            {
                Name = "Comp",
                Simple = new SimpleStruct
                {
                    Name = "Theon",
                    Age = 40,
                    Score = 79.5,
                    ExamDate = new DateTime(2016, 4, 20),
                    Pass = true
                }
            };

            var json = JsonMapper.NewContext().UseDateFormat(string.Empty).ToJson(complicated);

            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Comp", (string)jsonObj.Name);
            Assert.Equal("Theon", (string)jsonObj.Simple.Name);
            Assert.Equal(40, (int)jsonObj.Simple.Age);
            Assert.Equal(79.5, (double)jsonObj.Simple.Score);
            Assert.True((bool)jsonObj.Simple.Pass);
            DateTime dt;
            Assert.True(DateTime.TryParse((string)jsonObj.Simple.ExamDate, out dt));
            Assert.Equal(2016, dt.Year);
            Assert.Equal(4, dt.Month);
            Assert.Equal(20, dt.Day);
        }

        [Fact]
        public void TestMapObjectToJson33()
        {
            var complicated = new Complicated
            {
                Name = "Comp",
                Simple = new SimpleStruct
                {
                    Name = "Theon",
                    Age = 40,
                    Score = 79.5,
                    ExamDate = new DateTime(2016, 4, 20),
                    Pass = true
                }
            };

            var json = JsonMapper.NewContext().UseDateFormat(string.Empty).ToJson(complicated);

            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Comp", (string)jsonObj.Name);
            Assert.Equal("Theon", (string)jsonObj.Simple.Name);
            Assert.Equal(40, (int)jsonObj.Simple.Age);
            Assert.Equal(79.5, (double)jsonObj.Simple.Score);
            Assert.True((bool)jsonObj.Simple.Pass);
            DateTime dt;
            Assert.True(DateTime.TryParse((string)jsonObj.Simple.ExamDate, out dt));
            Assert.Equal(2016, dt.Year);
            Assert.Equal(4, dt.Month);
            Assert.Equal(20, dt.Day);

        }

        [Fact]
        public void TestMapObjectToJson34()
        {
            var guid = Guid.NewGuid();
            var json = JsonMapper.ToJson(guid);
            Assert.Equal(guid.ToString(), json.Trim('"'));
        }

        [Fact]
        public void TestMapObjectToJson35()
        {
            Guid? guid = Guid.NewGuid();
            var json = JsonMapper.ToJson(guid);
            Assert.Equal(guid.Value.ToString(), json.Trim('"'));
        }

        [Fact]
        public void TestMapObjectToJson36()
        {
            var list = new LinkedList<int>();
            for (var i = 0; i < 100; i++)
            {
                list.AddLast(i);
            }
            var obj = new LinkedListNested
            {
                Name = "Susan",
                Array = list
            };

            var json = JsonMapper.ToJson(obj);

            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Susan", (string)jsonObj.Name);
            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(i, (int)jsonObj.Array[i]);
            }
        }

        [Fact]
        public void TestMapObjectToJson37()
        {
            var anonymous = new { F1 = "V1", F2 = "V2", F3 = 10, F4 = new { F5 = 10.4, F6 = "V6" } };
            var json = JsonMapper.ToJson(anonymous);
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("V1", (string)jsonObj.F1);
            Assert.Equal("V2", (string)jsonObj.F2);
            Assert.Equal(10, (int)jsonObj.F3);
            Assert.Equal(10.4, (double)jsonObj.F4.F5);
            Assert.Equal("V6", (string)jsonObj.F4.F6);
        }

        [Fact]
        public void TestMapObjectToJson38()
        {
            int? number = 5;
            var json = JsonMapper.ToJson(number);
            Assert.Equal("5", json);
        }

		[Fact]
		public void TestMapObjectToJson39()
		{
			var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
			var bytes = new byte[10];
			rand.NextBytes(bytes);
			var str = Convert.ToBase64String(bytes);
			var array = new ByteArray
			{
				Bytes = bytes,
				Name = str
			};
			var json = JsonMapper.ToJson(array);
			var jsonObj = JsonMapper.Parse(json);
			Assert.Equal(str, (string)jsonObj.Bytes);
			Assert.Equal(str, (string)jsonObj.Name);
		}

        [Fact]
        public void TestMapProperty01()
        {
            var context = JsonMapper.NewContext().For<ToySet>(y => y.MapProperty(x => x.Name).With("ToyName")
                                    .MapProperty(x => x.ReleaseYear).With("Year")
                                    .MapProperty(x => x.Price).With("SellPrice")
                                    .MapProperty(x => x.SetNo).With("SetNumber"));
            var toy = new ToySet
            {
                Name = "Lego",
                ReleaseYear = 2015,
                SetNo = 42023,
                Price = 60.5,
                Category = "Technic",
                Producing = false
            };
            var json = context.ToJson(toy);
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
            var newToy = context.To<ToySet>(newJson);
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
            var context = JsonMapper.NewContext().For<Photo>(y => y.MapProperty(x => x.Location).With("Place").Not.MapProperty(x => x.Model));
            var photo = new Photo
            {
                Author = "Owen",
                Location = "Canada",
                Model = "EOS 5D Mark II",
                Time = new DateTime(2011, 5, 20)
            };
            var json = context.ToJson(photo);
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
            var context = JsonMapper.NewContext().For<Photo>(y => y.MapProperty(x => x.Location).With("Place").Not.MapProperty(x => x.Model));
            context.UseDateFormat("yyyy/MM/dd");
            var json = "{\"Author\": \"Owen\", \"Place\": \"France\", \"Model\": \"EOS 5D Mark II\", \"Time\": \"2011/05/30\"}";
            var photo = context.To<Photo>(json);
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
            var json = "{\"Person Name\": \"John\", \"Age\": 24}";
            var context = JsonMapper.NewContext().For<Person>(y => y.ConstructWith(() => new Person("UK", "Male")).MapProperty(x => x.Name).With("Person Name"));
            var person = context.To<Person>(json);
            Assert.Equal("John", person.Name);
            Assert.Equal(24, person.Age);
            Assert.Equal("UK", person.Nationality);
            Assert.Equal("Male", person.Gender);

            context.For<Person>(y => y.ConstructWith(() => new Person("USA", "Female")));
            json = "{\"Person Name\": \"Rose\", \"Age\": 24}";
            person = context.To<Person>(json);
            Assert.Equal("Rose", person.Name);
            Assert.Equal(24, person.Age);
            Assert.Equal("USA", person.Nationality);
            Assert.Equal("Female", person.Gender);
        }

        [Fact]
        public void TestMapProperty05()
        {
            var context = JsonMapper.NewContext();
            context.For<Person>(y => y.ConstructWith(() => new Person("FR", "Female")).MapProperty(x => x.Name).With("Person Name"));
            context.For<Student>(y => y.MapProperty(x => x.Person).With("Personal Information"));
            var json =
                "{\"Personal Information\": {\"Person Name\": \"Emily\", \"Age\": 21}, \"Major\": \"CS\", \"Grade\": 3}";
            var student = context.To<Student>(json);
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
            var hasDate = JsonMapper.NewContext().UseDateFormat("yyyy-MM-dd").To<HasDate>(json);
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
            var context = JsonMapper.NewContext().For<Person>(y => y.ConstructWith(() => new Person("JP", "Male")).MapProperty(x => x.Name).With("person name"));
            context.For<Student>(y => y.MapProperty(x => x.Person).With("personal information"));
            var json = context.ToJson(student);
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
            var newStudent = context.To<Student>(json);
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
            var hasDate = JsonMapper.NewContext().UseDateFormat("MM/dd/yyyy").To<HasDate>(json);
            Assert.Equal("Joe", hasDate.Name);
            Assert.Equal(1995, hasDate.Birthday.Year);
            Assert.Equal(10, hasDate.Birthday.Month);
            Assert.Equal(5, hasDate.Birthday.Day);
        }

        [Fact]
        public void TestMapProperty09()
        {
            var context = JsonMapper.NewContext().For<Person>(y =>
                y.ConstructWith(() => new Person("US", "Male"))
                .MapProperty(x => x.Name).With("Person Name"));
            var json =
                "[{\"Person Name\": \"Jackson\", \"Age\": 22}, {\"Person Name\": \"Johnson\", \"Age\": 21}, {\"Person Name\": \"Hugo\", \"Age\": 23}]";
            var people = context.To<List<Person>>(json);
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
        public void TestMapProperty10()
        {
            var json = "{\"Birthday\": \"10:05:1995\", \"Name\": \"Joe\"}";
            var hasDate = JsonMapper.NewContext().UseDateFormat("MM:dd:yyyy").To<HasDate>(json);
            Assert.Equal("Joe", hasDate.Name);
            Assert.Equal(1995, hasDate.Birthday.Year);
            Assert.Equal(10, hasDate.Birthday.Month);
            Assert.Equal(5, hasDate.Birthday.Day);

            Assert.Throws(typeof(ArgumentException), () => JsonMapper.NewContext().UseDateFormat("yyyy/mm/dd").To<HasDate>(json));
        }
    }
}
