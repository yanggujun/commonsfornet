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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

using Newtonsoft.Json;

using Commons.Json;
using Commons.Test.Json;

namespace Commons.Perf
{
	// TODO: 
	//    large byte array buffer
	public class Program
    {
        public static void Main(string[] args)
        {
            Test();
        }

        public static void Test()
        {
			TestTrivialObjectToJson();
			TestSmallObjectToJson();
			TestNormalObjectToJson();
			TestStandardObjectToJson();
			TestLargeObjectToJson();

			TestTrivialObjectJsonToObject();
			TestSmallObjectJsonToObject();
			TestStandardObjectJsonToObject();
			TestLargeObjectJsonToObject();
        }

		public static void TestTrivialObjectToJson()
		{
			const int LN = 1000000;
			var warm = Message.Create(0);
			JsonMapper.ToJson(warm);
			JsonConvert.SerializeObject(warm);
			var sw1 = new Stopwatch();
			var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				var message = Message.Create(i + 1);
				sw1.Start();
				JsonMapper.ToJson(message);
				sw1.Stop();
			}

			for (var i = 0; i < LN; i++)
			{
				var message = Message.Create(i + 1);
				sw2.Start();
				JsonConvert.SerializeObject(message);
				sw2.Stop();
			}

			Console.WriteLine("------------------");
            Console.WriteLine("Trivial object to Json");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
		}

		public static void TestTrivialObjectJsonToObject()
		{
			const int LN = 1000000;
			var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
			var template = ReadFile("Message.txt");
			var warm = GenerateMessageJson(rand, template);
			JsonMapper.To<Message>(warm);
			JsonConvert.DeserializeObject<Message>(warm);

			var sw1 = new Stopwatch();
			var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				var json = GenerateMessageJson(rand, template);
				sw1.Start();
				JsonMapper.To<Message>(json);
				sw1.Stop();
			}

			for (var i = 0; i < LN; i++)
			{
				var json = GenerateMessageJson(rand, template);
				sw2.Start();
				JsonConvert.DeserializeObject<Message>(json);
				sw2.Stop();
			}
			Console.WriteLine("------------------");
            Console.WriteLine("Trivial object Json To Object");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
		}

		public static void TestLargeObjectToJson()
		{
			const int LN = 10000;
            var warm = Book.Factory<Book, Genre, Page, Headnote, Footnote>(7, x => (Genre)x);
			JsonMapper.ToJson(warm);
			JsonConvert.SerializeObject(warm);

			var sw1 = new Stopwatch();
			var sw2 = new Stopwatch();

			for (var i = 0; i < LN; i++)
			{
				var book = Book.Factory<Book, Genre, Page, Headnote, Footnote>(7, x => (Genre)x);
				sw1.Start();
				JsonMapper.ToJson(book);
				sw1.Stop();
			}

			GC.Collect();
			Thread.Sleep(1000);

			for (var i = 0; i < LN; i++)
			{
				var book = Book.Factory<Book, Genre, Page, Headnote, Footnote>(7, x => (Genre)x);
				sw2.Start();
				JsonConvert.SerializeObject(book);
				sw2.Stop();
			}

			Console.WriteLine("------------------");
            Console.WriteLine("Large object to Json");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
		}

        public static void TestStandardObjectToJson()
        {
            const int LN = 100000;

            var warm = Post.Factory<Post, Vote, PostState, Comment>((int)(0x0000ffff & DateTime.Now.Ticks), x => (PostState)x);
            JsonMapper.ToJson(warm);
            JsonConvert.SerializeObject(warm);

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
				sw1.Start();
				JsonMapper.ToJson(post);
				sw1.Stop();
			}

            for (var i = 0; i < LN; i++)
            {
                var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
                sw2.Start();
                JsonConvert.SerializeObject(post);
                sw2.Stop();
            }

			Console.WriteLine("------------------");
            Console.WriteLine("Standard object to Json");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
        }

        public static void TestSmallObjectToJson()
        {
            const int LN = 1000000;
            var warm = SmallPost.Create((int)DateTime.Now.Ticks & 0x0000ffff);
            JsonMapper.ToJson(warm);
            JsonConvert.SerializeObject(warm);
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				var p = SmallPost.Create(i);
				sw1.Start();
				JsonMapper.ToJson(p);
				sw1.Stop();
			}

            for (var i = 0; i < LN; i++)
            {
				var p = SmallPost.Create(i);
                sw2.Start();
                JsonConvert.SerializeObject(p);
                sw2.Stop();
            }


			Console.WriteLine("------------------");
            Console.WriteLine("Small object to Json");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
        }

		public static void TestNormalObjectToJson()
		{
			const int LN = 1000000;
			var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
			var warm = CompletePrimitiveObject.Factory(rand);
			JsonConvert.SerializeObject(warm);
			JsonMapper.ToJson(warm);

			var sw1 = new Stopwatch();
			var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				var obj = CompletePrimitiveObject.Factory(rand);
				sw1.Start();
				JsonMapper.ToJson(obj);
				sw1.Stop();
			}

			for (var i = 0; i < LN; i++)
			{
				var obj = CompletePrimitiveObject.Factory(rand);
				sw2.Start();
				JsonConvert.SerializeObject(obj);
				sw2.Stop();
			}
			Console.WriteLine("------------------");
            Console.WriteLine("Complete Primitive Object to Json");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
		}

        public static void TestSmallObjectJsonToObject()
        {
            const int LN = 1000000;

			var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));

			var jsonTemplate = ReadFile("SmallPost.txt");
			var warm = GenerateSmallPostJson(rand, jsonTemplate);
            JsonMapper.To<SmallPost>(warm);
            JsonConvert.DeserializeObject<SmallPost>(warm);

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				var json = GenerateSmallPostJson(rand, jsonTemplate);
				sw1.Start();
				JsonMapper.To<SmallPost>(json);
				sw1.Stop();
			}

            for (var i = 0; i < LN; i++)
            {
				var json = GenerateSmallPostJson(rand, jsonTemplate);
                sw2.Start();
                JsonConvert.DeserializeObject<SmallPost>(json);
                sw2.Stop();
            }

			Console.WriteLine("------------------");
            Console.WriteLine("Json to small object");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
        }

        public static void TestStandardObjectJsonToObject()
        {
			var json = ReadFile("Post.txt");
			JsonMapper.To<Post>(json);
			JsonConvert.DeserializeObject<Post>(json);

            const int LN = 10000;

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				sw1.Start();
				JsonMapper.To<Post>(json);
				sw1.Stop();
			}

            for (var i = 0; i < LN; i++)
            {
                sw2.Start();
                JsonConvert.DeserializeObject<Post>(json);
                sw2.Stop();
            }

			Console.WriteLine("------------------");
            Console.WriteLine("Json to standard object");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
        }

		public static void TestLargeObjectJsonToObject()
		{
			//var context = JsonMapper.NewContext().UseDateFormat("MM/dd/yyyy HH:mm:ss.fff").For<Note>(y => y.ConstructWith(x =>
			//{
			//	var jsonObj = x as JObject;
			//	Note note;
			//	if (jsonObj.ContainsKey("index"))
			//	{
			//		var foot = new Footnote();
			//		foot.note = jsonObj.GetString("note");
			//		foot.writtenBy = jsonObj.GetString("writtenBy");
			//		foot.index = jsonObj.GetInt64("index");
			//		foot.createadAt = DateTime.Parse(jsonObj.GetString("createadAt"));
			//		note = foot;
			//	}
			//	else
			//	{
			//		var head = new Headnote();
			//		head.note = jsonObj.GetString("note");
			//		head.writtenBy = jsonObj.GetString("writtenBy");
			//		head.modifiedAt = DateTime.Parse(jsonObj.GetString("modifiedAt"));
			//		note = head;
			//	}
			//	return note;
			//}));
			var json = ReadFile("NewBook.txt");
			JsonMapper.To<NewBook>(json);
			JsonConvert.DeserializeObject<NewBook>(json);

            const int LN = 10000;

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
			for (var i = 0; i < LN; i++)
			{
				sw1.Start();
				JsonMapper.To<NewBook>(json);
				sw1.Stop();
			}

			GC.Collect();
			Thread.Sleep(1000);

            for (var i = 0; i < LN; i++)
            {
                sw2.Start();
                JsonConvert.DeserializeObject<NewBook>(json);
                sw2.Stop();
            }

			Console.WriteLine("------------------");
            Console.WriteLine("Json to large object");
            PrintResult("JsonMapper", sw1.ElapsedMilliseconds, "Json.NET", sw2.ElapsedMilliseconds);
			Console.WriteLine("------------------");
		}

        private static void PrintResult(string test1, long test1Ms, string test2, long test2Ms)
        {
            string faster, slower;
            double rate;
            if (test1Ms > test2Ms)
            {
                faster = test2;
                slower = test1;
                rate = ((double)test1Ms - (double)test2Ms) / (double)test2Ms;
            }
            else
            {
                faster = test1;
                slower = test2;
                rate = ((double)test2Ms - (double)test1Ms) / (double)test1Ms;
            }
            rate = rate * 100;
            rate = Math.Round(rate, 2);

            var result = string.Format("{0}: {1} \n{2}: {3}", test1, test1Ms, test2, test2Ms);
            var comp = string.Format("{0} is slower than {1} by {2}%", slower, faster, rate);
            Console.WriteLine(result);
            Console.WriteLine(comp);
            
        }

		private static string ReadFile(string name)
		{
			string result;
			using (var fs = new FileStream(name, FileMode.Open))
			{
				using (var sr = new StreamReader(fs))
				{
					result = sr.ReadToEnd();
				}
			}
			return result;
		}

		private static string GenerateSmallPostJson(Random rand, string template)
		{
			var result = template.Replace("(GUID)", Guid.NewGuid().ToString());
			result = result.Replace("(TITLE)", "Some title" + rand.Next());
			result = result.Replace("(ACTIVE)", (rand.Next() % 2 == 0).ToString().ToLower());
			result = result.Replace("(CREATED)", DateTime.Now.ToString(CultureInfo.InvariantCulture));
			result = result.Replace("(COUNT)", (rand.Next() % 1000).ToString());

			return result;
		}

		private static string GenerateMessageJson(Random rand, string template)
		{
			var result = template.Replace("(MESSAGE)", "Some message " + rand.Next());
			result = result.Replace("(VERSION)", rand.Next().ToString());
			return result;
		}
    }
}
