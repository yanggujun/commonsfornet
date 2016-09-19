using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Commons.Json;
using Commons.Test.Json;
using Newtonsoft.Json;

namespace Commons.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestSmall();
        }

        public static void TestStandard()
        {
            const int LN = 100000;

            var sw1 = new Stopwatch();
            for (var i = 0; i < LN; i++)
            {
                var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
                sw1.Start();
                JsonMapper.ToJson(post);
                sw1.Stop();
            }

            var sw2 = new Stopwatch();
            for (var i = 0; i < LN; i++)
            {
                var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
                sw2.Start();
                JsonConvert.SerializeObject(post);
                sw2.Stop();
            }

            Console.WriteLine("Standard object ------ ");
            Console.WriteLine("JsonMapper: " + sw1.ElapsedMilliseconds);
            Console.WriteLine("Json.NET " + sw2.ElapsedMilliseconds);
        }

        public static void TestSmall()
        {
            const int LN = 1000000;
            var sw1 = new Stopwatch();
            for (var i = 0; i < LN; i++)
            {
                var p = SmallPost.Create(i);
                sw1.Start();
                JsonMapper.ToJson(p);
                sw1.Stop();
            }

            var sw2 = new Stopwatch();
            for (var i = 0; i < LN; i++)
            {
                var p = SmallPost.Create(i);
                sw2.Start();
                JsonConvert.SerializeObject(p);
                sw2.Stop();
            }

            Console.WriteLine("Small object ----- ");
            Console.WriteLine("JsonMapper: " + sw1.ElapsedMilliseconds);
            Console.WriteLine("Json.NET: " + sw2.ElapsedMilliseconds);
        }
    }
}
