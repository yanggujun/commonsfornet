using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Commons.Json;
using Commons.Test.Json;
using Newtonsoft.Json;
using Xunit;

namespace Commons.Perf
{
    public class JsonPerformanceTest
    {
        private const int TestNumber = 100000;

        [Fact]
        public void TestSerializeJsonSmall()
        {
            var list = new List<SmallPost>();
            for (var i = 0; i < TestNumber; i++)
            {
                var p = SmallPost.Create(i);
                list.Add(p);
            }

            var sw1 = new Stopwatch();
            sw1.Start();
            foreach (var p in list)
            {
                JsonMapper.ToJson(p);
            }
            sw1.Stop();

            var sw2 = new Stopwatch();
            sw2.Start();
            foreach (var p in list)
            {
                JsonConvert.SerializeObject(p);
            }
            sw2.Stop();

            Console.WriteLine("Small object ----- ");
            Console.WriteLine("JsonMapper: " + sw1.ElapsedMilliseconds);
            Console.WriteLine("Json.NET: " + sw2.ElapsedMilliseconds);
        }

        [Fact]
        public void TestSerializeJsonStandard()
        {
            var list = new List<Post>();
            for (var i = 0; i < TestNumber; i++)
            {
                var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
                list.Add(post);
            }

            var sw1 = new Stopwatch();
            sw1.Start();
            foreach (var p in list)
            {
                JsonMapper.ToJson(p);
            }
            sw1.Stop();

            var sw2 = new Stopwatch();
            sw2.Start();
            foreach (var p in list)
            {
                JsonConvert.SerializeObject(p);
            }
            sw2.Stop();

            Console.WriteLine("Standard object ------ ");
            Console.WriteLine("JsonMapper: " + sw1.ElapsedMilliseconds);
            Console.WriteLine("Json.NET " + sw2.ElapsedMilliseconds);
        }
    }
}
