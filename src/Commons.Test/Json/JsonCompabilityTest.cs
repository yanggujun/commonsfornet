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
        public void TestCompability001()
        {
            var json = "{\"\\uDFAA\": 0}";
            JsonMapper.To<Dictionary<string, int>>(json);
        }

        [Fact]
        public void TestCompability002()
        {
            var json = "[\"\\uDADA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability003()
        {
            var json = "[\"\\uD888\\u1234\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability004()
        {
            var json = "[\"\\uD800\\uD800\n\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability005()
        {
            var json = "[\"\\uDd1ea\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability006()
        {
            var json = "[\"\\ud800\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability007()
        {
            var json = "[\"\\ud800abc\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability008()
        {
            var json = "[\"\\uDd1e\uD834\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability009()
        {
            var json = "[\"\\\uDFAA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability010()
        {
            var json = "[\"\\\uDFAA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability011()
        {
            var json = "[1 true]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability012()
        {
            var json = "[\"\":1]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, int>>(json));
        }

        [Fact]
        public void TestCompability013()
        {
            var json = "[\"\"],";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability014()
        {
            var json = "[,1]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability015()
        {
            var json = "[1,,2]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability016()
        {
            var json = "[\"x\",,]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability017()
        {
            var json = "[\"x\"]]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability018()
        {
            var json = "[\"\",]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability019()
        {
            var json = "[\"x\"";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability020()
        {
            var json = "[x";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability021()
        {
            var json = "[3[4]]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability022()
        {
            var json = "[1:2]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability023()
        {
            var json = "[,]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability024()
        {
            var json = "[-]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability025()
        {
            var json = "[   ,\"\"]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability026()
        {
            var json = "[\"a\", \n4\n,1,";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability027()
        {
            var json = "[1,";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability028()
        {
            var json = "[1, \n1\n,1";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability029()
        {
            var json = "[{}";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability030()
        {
            var json = "[fals]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability031()
        {
            var json = "[nul]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability032()
        {
            var json = "[tru]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<bool[]>(json));
        }

        //[Fact]
        //public void TestCompability033()
        //{
        //    var json = "123";
        //    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int>(json));
        //}

        [Fact]
        public void TestCompability034()
        {
            var json = "[++1234]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability035()
        {
            var json = "[+1]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability036()
        {
            var json = "[+inf]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        //[Fact]
        public void TestCompability037()
        {
            var json = "[-01]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability038()
        {
            var json = "[-1.0.]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability039()
        {
            var json = "[-2.]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability040()
        {
            var json = "[-NaN]";
            Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability041()
        {
	        var json = "[.-1]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
        }

		[Fact]
	    public void TestCompability042()
		{
			var json = "[.2e-3]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability043()
		{
			var json = "[0.1.2]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}

		[Fact]
	    public void TestCompability044()
		{
			var json = "[0.3e+]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}

		[Fact]
	    public void TestCompability045()
		{
			var json = "[0.3e]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}

		[Fact]
	    public void TestCompability046()
		{
			var json = "[0.e1]";
			Assert.Throws(typeof(InvalidCastException), () => JsonMapper.To<double[]>(json));
		}

		[Fact]
	    public void TestCompability047()
		{
			var json = "[0E+]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability048()
		{
			var json = "[0E]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability049()
		{
			var json = "[0e+]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability050()
		{
			var json = "[0e]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability051()
		{
			var json = "[1.0e+]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability052()
		{
			var json = "[1.0e-]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability053()
		{
			var json = "[1.0e]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability054()
		{
			var json = "[1 000.0]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}
		[Fact]
	    public void TestCompability055()
		{
			var json = "[2.e+3]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability056()
		{
			var json = "[1eE2]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability057()
		{
			var json = "[2.e-3]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability058()
		{
			var json = "[2.e3]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability059()
		{
			var json = "[9.e+]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability060()
		{
			var json = "[Inf]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability061()
		{
			var json = "[NaN]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability062()
		{
			var json = "[1+2]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability063()
		{
			var json = "[0x1]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability064()
		{
			var json = "[0x42]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability065()
		{
			var json = "[Infinity]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability066()
		{
			var json = "[0e+-1]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability067()
		{
			var json = "[-123.123foo]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}
		[Fact]
	    public void TestCompability068()
		{
			var json = "[-Infinity]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability069()
		{
			var json = "[-foo]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		[Fact]
	    public void TestCompability070()
		{
			var json = "[- 1]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}
		// n_number_minus_space_1.json

		//[Fact]
	    public void TestCompability071()
		{
			var json = "[-012]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		//[Fact]
	    public void TestCompability072()
		{
			var json = "[-.123]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability073()
		{
			var json = "[-1x]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability074()
		{
			var json = "[1ea]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability075()
		{
			var json = "[1.]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}

		[Fact]
	    public void TestCompability076()
	    {
		    var json = "[.123]";
		    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
	    }

		[Fact]
	    public void TestCompability077()
		{
			var json = "[1.2a-3]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability078()
		{
			var json = "[1.8011670033376514H-308]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<double[]>(json));
		}

		//[Fact]
	    public void TestCompability079()
		{
			var json = "[012]";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<int[]>(json));
		}

		[Fact]
	    public void TestCompability080()
	    {
			var json = "[\"x\", truth]";
		    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<string[]>(json));
	    }

		[Fact]
	    public void TestCompability081()
	    {
		    var json = "{[:\"x\"";
		    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
	    }

		[Fact]
	    public void TestCompability082()
	    {
		    var json = "{\"x\", null}";
		    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
	    }

		[Fact]
		public void TestCompability083()
		{
			var json = "{\"x\"::\"b\"";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}

		[Fact]
	    public void TestCompability084()
	    {
		    var json = "{CH}";
		    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
	    }

		[Fact]
	    public void TestCompability085()
		{
			var json = "{\"a\":\"a\" 123}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}

	    [Fact]
	    public void TestCompability086()
	    {
		    var json = "{key: 'value'}";
		    Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
	    }

		[Fact]
	    public void TestCompability087()
		{
			var json = "{\"a\" b}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}// n_object_missing_colon

		[Fact]
		public void TestCompability088()
		{
			var json = "{\"a\": \"\"}";
			var dict = JsonMapper.To<Dictionary<string, string>>(json);
			Assert.Equal(string.Empty, dict["a"]);
		}

		[Fact]
		public void TestCompability089()
		{
			var json = "{\"a\": }";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}

		[Fact]
		public void TestCompability090()
		{
			var json = "[]";
			var array = JsonMapper.To<int[]>(json);
			Assert.Equal(0, array.Length);
		}

		[Fact]
		public void TestCompability091()
		{
			var json = "{\"a:\"b\"}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}

		[Fact]
		public void TestCompability092()
		{
			var json = "{\"a\":b\"}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}

		[Fact]
		public void TestCompability093()
		{
			var json = "{\"a\":\"b}";
			Assert.Throws(typeof(ArgumentException), () => JsonMapper.To<Dictionary<string, string>>(json));
		}

	}
}
