using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.Json;
using Xunit;

namespace Test.Commons.Json
{
    public class JsonCompabilityTest
    {
        [Fact]
        public void TestCompability01()
        {
            var json = "{\"\\uDFAA\": 0}";
            JsonMapper.To<Dictionary<string, int>>(json);
        }

        [Fact]
        public void TestCompability02()
        {
            var json = "[\"\\uDADA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability03()
        {
            var json = "[\"\\uD888\\u1234\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability04()
        {
            var json = "[\"\\uD800\\uD800\n\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability05()
        {
            var json = "[\"\\uDd1ea\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability06()
        {
            var json = "[\"\\ud800\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability07()
        {
            var json = "[\"\\ud800abc\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability08()
        {
            var json = "[\"\\uDd1e\uD834\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability09()
        {
            var json = "[\"\\\uDFAA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability10()
        {
            var json = "[\"\\\uDFAA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability11()
        {
            var json = "[1 true]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability12()
        {
            var json = "[\"\":1]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, int>>(json));
        }

        [Fact]
        public void TestCompability13()
        {
            var json = "[\"\"],";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability14()
        {
            var json = "[,1]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability15()
        {
            var json = "[1,,2]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability16()
        {
            var json = "[\"x\",,]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability17()
        {
            var json = "[\"x\"]]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability18()
        {
            var json = "[\"\",]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability19()
        {
            var json = "[\"x\"";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability20()
        {
            var json = "[x";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability21()
        {
            var json = "[3[4]]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability22()
        {
            var json = "[1:2]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability23()
        {
            var json = "[,]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability24()
        {
            var json = "[-]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability25()
        {
            var json = "[   ,\"\"]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability26()
        {
            var json = "[\"a\", \n4\n,1,";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }
    }
}
