using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Commons.Json;
using Commons.Json.Mapper;
using Commons.Test.Json;
using Newtonsoft.Json;

namespace Commons.Perf
{
	// TODO: 
	// byte[]
	//    large byte array buffer
	// change configuration behavior
	// Json2Object
	//    initialize list with an size.
    public class Program
    {
        public static void Main(string[] args)
        {
            //TestSmallObjectToJson();
			//TestNormalObjectToJson();
            //TestStandardObjectToJson();
			//TestLargeObjectToJson();
            TestSmallObjectJsonToObject();
            //TestStandardObjectJsonToObject();
        }

		public static void TestMisc()
		{
			var sw1 = new Stopwatch();
			sw1.Start();
			for (var i = 0; i < 1000000; i++)
			{
				var h = Guid.NewGuid().ToString().ToLower().GetHashCode();
			}
		}

		public static void TestLargeObjectToJson()
		{
			const int LN = 10000;
            //JsonMapper.UseDateFormat("MM/dd/yyyy HH:mm:ss").For<Note>().ConstructWith(x =>
            //{
            //    var jsonObj = x as JObject;
            //    Note note;
            //    if (jsonObj.ContainsKey("index"))
            //    {
            //        var foot = new Footnote();
            //        foot.note = jsonObj.GetString("note");
            //        foot.writtenBy = jsonObj.GetString("writtenBy");
            //        foot.index = jsonObj.GetInt64("index");
            //        foot.createadAt = DateTime.Parse(jsonObj.GetString("createadAt"));
            //        note = foot;
            //    }
            //    else
            //    {
            //        var head = new Headnote();
            //        head.note = jsonObj.GetString("note");
            //        head.writtenBy = jsonObj.GetString("writtenBy");
            //        head.modifiedAt = DateTime.Parse(jsonObj.GetString("modifiedAt"));
            //        note = head;
            //    }
            //    return note;
            //});
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

				sw2.Start();
				JsonConvert.SerializeObject(warm);
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
            var warm = SmallPost.Create((int)DateTime.Now.Ticks & 0x0000ffff);
            var jsonWarm = JsonMapper.ToJson(warm);
            JsonMapper.To<SmallPost>(jsonWarm);
            JsonConvert.DeserializeObject<SmallPost>(jsonWarm);

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            for (var i = 0; i < LN; i++)
            {
                var p = SmallPost.Create(i);
                var json = JsonMapper.ToJson(p);

                sw1.Start();
                JsonMapper.To<SmallPost>(json);
                sw1.Stop();

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
            const int LN = 100000;
            var warm = Post.Factory<Post, Vote, PostState, Comment>((int)(0x0000ffff & DateTime.Now.Ticks), x => (PostState)x);
            var jsonWarm = JsonMapper.ToJson(warm);
            JsonMapper.To<Post>(jsonWarm);
            JsonConvert.DeserializeObject<Post>(jsonWarm);

            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            for (var i = 0; i < LN; i++)
            {
                var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
                var json = JsonMapper.ToJson(post);

                sw1.Start();
                JsonMapper.To<Post>(json);
                sw1.Stop();

                sw2.Start();
                JsonConvert.DeserializeObject<Post>(json);
                sw2.Stop();
            }

			Console.WriteLine("------------------");
            Console.WriteLine("Json to standard object");
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
    }
}
