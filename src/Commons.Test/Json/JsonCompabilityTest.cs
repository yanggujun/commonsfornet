using Commons.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace Commons.Test.Json
{
    public class JsonCompabilityTest
    {
        //[Fact]
        public void TestCompability001()
        {
            var json = "{\"\\uDFAA\": 0}";
            JsonMapper.To<Dictionary<string, int>>(json);
        }

        //[Fact]
        public void TestCompability002()
        {
            var json = "[\"\\uDADA\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability003()
        {
            var json = "[\"\\uD888\\u1234\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability004()
        {
            var json = "[\"\\uD800\\uD800\n\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability005()
        {
            var json = "[\"\\uDd1ea\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability006()
        {
            var json = "[\"\\ud800\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability007()
        {
            var json = "[\"\\ud800abc\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability008()
        {
            var json = "[\"\\uDd1e\uD834\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability009()
        {
            var json = "[\"\\\uDFAA\"]";
            JsonMapper.To<string[]>(json);
        }

        //[Fact]
        public void TestCompability010()
        {
            var json = "[\"\\\uDFAA\"]";
            JsonMapper.To<string[]>(json);
        }

        [Fact]
        public void TestCompability011()
        {
            var json = "[1 true]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability012()
        {
            var json = "[\"\":1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, int>>(json));
        }

        [Fact]
        public void TestCompability013()
        {
            var json = "[\"\"],";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability014()
        {
            var json = "[,1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability015()
        {
            var json = "[1,,2]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability016()
        {
            var json = "[\"x\",,]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability017()
        {
            var json = "[\"x\"]]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability018()
        {
            var json = "[\"\",]";
            var array = JsonMapper.To<string[]>(json);
            Assert.Single(array);
            Assert.Equal(string.Empty, array[0]);
        }

        [Fact]
        public void TestCompability019()
        {
            var json = "[\"x\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability020()
        {
            var json = "[x";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability021()
        {
            var json = "[3[4]]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability022()
        {
            var json = "[1:2]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability023()
        {
            var json = "[,]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability024()
        {
            var json = "[-]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability025()
        {
            var json = "[   ,\"\"]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability026()
        {
            var json = "[\"a\", \n4\n,1,";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability027()
        {
            var json = "[1,";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability028()
        {
            var json = "[1, \n1\n,1";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability029()
        {
            var json = "[{}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability030()
        {
            var json = "[fals]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability031()
        {
            var json = "[nul]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability032()
        {
            var json = "[tru]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability034()
        {
            var json = "[++1234]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability035()
        {
            var json = "[+1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability036()
        {
            var json = "[+inf]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability037()
        {
            var json = "[-01]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability038()
        {
            var json = "[-1.0.]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability039()
        {
            var json = "[-2.]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability040()
        {
            var json = "[-NaN]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability041()
        {
            var json = "[.-1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability042()
        {
            var json = "[.2e-3]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability043()
        {
            var json = "[0.1.2]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability044()
        {
            var json = "[0.3e+]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability045()
        {
            var json = "[0.3e]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability046()
        {
            var json = "[0.e1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability047()
        {
            var json = "[0E+]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability048()
        {
            var json = "[0E]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability049()
        {
            var json = "[0e+]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability050()
        {
            var json = "[0e]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability051()
        {
            var json = "[1.0e+]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability052()
        {
            var json = "[1.0e-]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability053()
        {
            var json = "[1.0e]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability054()
        {
            var json = "[1 000.0]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }
        [Fact]
        public void TestCompability055()
        {
            var json = "[2.e+3]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability056()
        {
            var json = "[1eE2]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability057()
        {
            var json = "[2.e-3]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability058()
        {
            var json = "[2.e3]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability059()
        {
            var json = "[9.e+]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability060()
        {
            var json = "[Inf]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability061()
        {
            var json = "[NaN]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability062()
        {
            var json = "[1+2]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability063()
        {
            var json = "[0x1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability064()
        {
            var json = "[0x42]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability065()
        {
            var json = "[Infinity]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability066()
        {
            var json = "[0e+-1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability067()
        {
            var json = "[-123.123foo]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }
        [Fact]
        public void TestCompability068()
        {
            var json = "[-Infinity]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability069()
        {
            var json = "[-foo]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        [Fact]
        public void TestCompability070()
        {
            var json = "[- 1]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }
        // n_number_minus_space_1.json

        [Fact]
        public void TestCompability071()
        {
            var json = "[-012]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability072()
        {
            var json = "[-.123]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability073()
        {
            var json = "[-1x]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability074()
        {
            var json = "[1ea]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability075()
        {
            var json = "[1.]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability076()
        {
            var json = "[.123]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability077()
        {
            var json = "[1.2a-3]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability078()
        {
            var json = "[1.8011670033376514H-308]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double[]>(json));
        }

        [Fact]
        public void TestCompability079()
        {
            var json = "[012]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability080()
        {
            var json = "[\"x\", truth]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability081()
        {
            var json = "{[:\"x\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability082()
        {
            var json = "{\"x\", null}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability083()
        {
            var json = "{\"x\"::\"b\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability084()
        {
            var json = "{CH}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability085()
        {
            var json = "{\"a\":\"a\" 123}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability086()
        {
            var json = "{key: 'value'}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability087()
        {
            var json = "{\"a\" b}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
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
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
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
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability092()
        {
            var json = "{\"a\":b\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability093()
        {
            var json = "{\"a\":\"b}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability094()
        {
            var json = "[5, ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability095()
        {
            var json = "{\"a\":\"b\",}";
            var result = JsonMapper.To<Dictionary<string, string>>(json);
            Assert.Equal("b", result["a"]);
        }

        [Fact]
        public void TestCompability096()
        {
            var json = "{\"a\":\"b\", ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability097()
        {
            var json = "[5, )";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability098()
        {
            var json = "[5}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability099()
        {
            var json = "{\"a\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability100()
        {
            var json = "{\"a\":}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability101()
        {
            var json = "[5,] ";
            var array = JsonMapper.To<int[]>(json);
            Assert.Equal(1, array.Length);
            Assert.Equal(5, array[0]);
        }

        [Fact]
        public void TestCompability102()
        {
            var json = "    [    5,    ]    ";
            var array = JsonMapper.To<int[]>(json);
            Assert.Equal(1, array.Length);
            Assert.Equal(5, array[0]);
        }

        [Fact]
        public void TestCompability103()
        {
            var json = " [ 5   ,   ]";
            var array = JsonMapper.To<int[]>(json);
            Assert.Equal(1, array.Length);
            Assert.Equal(5, array[0]);
        }

        [Fact]
        public void TestCompability104()
        {
            var json = " [ 5   ,   ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability105()
        {
            var json = "tre";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool>(json));
        }

        [Fact]
        public void TestCompability106()
        {
            var json = "  true   ";
            var b = JsonMapper.To<bool>(json);
            Assert.True(b);
        }

        [Fact]
        public void TestCompability107()
        {
            var json = "   tr  ue  ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool>(json));
        }

        [Fact]
        public void TestCompability108()
        {
            var json = "0";
            var v = JsonMapper.To<int>(json);
            Assert.Equal(0, v);
        }

        [Fact]
        public void TestCompability109()
        {
            var json = "0.";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double>(json));
        }

        [Fact]
        public void TestCompability110()
        {
            var json = "0a";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int>(json));
        }

        [Fact]
        public void TestCompability111()
        {
            var json = "0.12a";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double>(json));
        }

        [Fact]
        public void TestCompability112()
        {
            var json = "0   ";
            var v = JsonMapper.To<int>(json);
            Assert.Equal(0, v);
        }

        [Fact]
        public void TestCompability113()
        {
            var json = "0   20   ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int>(json));
        }

        [Fact]
        public void TestCompability114()
        {
            var json = "0.b12";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<double>(json));
        }

        [Fact]
        public void TestCompability115()
        {
            var json = "0.12e+5";
            double expected = 0.12e+5;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability116()
        {
            var json = "1e5";
            double expected = 1e5;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability117()
        {
            var json = "1E5";
            double expected = 1E5;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability118()
        {
            var json = "3.2e2";
            double expected = 3.2e2;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability119()
        {
            var json = "3.2e+24";
            double expected = 3.2e+24;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability120()
        {
            var json = "3.2E2";
            double expected = 3.2E2;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability121()
        {
            var json = "3.2E+24";
            double expected = 3.2E+24;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability122()
        {
            var json = "3.2E-24";
            double expected = 3.2E-24;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability123()
        {
            var json = "3.2e-24";
            double expected = 3.2e-24;
            var result = JsonMapper.To<double>(json);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompability124()
        {
            var json = "{:\"b\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability125()
        {
            var json = "{\"a\" \"b\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability126()
        {
            var json = "{\"a\":";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability127()
        {
            var json = "{\"a\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability128()
        {
            var json = "{1:1}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability129()
        {
            var json = "{9999E9999:1}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability130()
        {
            var json = "{null:null,null:null}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability131()
        {
            var json = "{\"id\":0,,,,}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability132()
        {
            var json = "{'a':0}";
            var dict = JsonMapper.To<Dictionary<string, int>>(json);
            Assert.Equal(0, dict["a"]);
        }

        [Fact]
        public void TestCompability133()
        {
            var json = "{\"a\":0}/**/";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability134()
        {
            var json = "{\"a\":0}/**//";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability135()
        {
            var json = "{\"a\":0}//";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }
        
        [Fact]
        public void TestCompability136()
        {
            var json = "{\"a\":0}/";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability137()
        {
            var json = "{\"a\":b,,\"c\":\"d\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability138()
        {
            var json = "{a:\"b\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }
        [Fact]
        public void TestCompability139()
        {
            var json = "{\"a\":0}/**/";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability140()
        {
            var json = "{\"a\":\"a";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability141()
        {
            var json = "{\"foo\":\"bar\", \"a\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability142()
        {
            var json = "{\"a\":\"b\"}#";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability143()
        {
            var json = " ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        // TODO: missing from n_string_backslash_00
        //               to   n_string_no_quotes_with_bad_escape

        [Fact]
        public void TestCompability144()
        {
            var json = "\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string>(json));
        }

        [Fact]
        public void TestCompability145()
        {
            var json = "['single quote']";
            var strings = JsonMapper.To<string[]>(json);
            Assert.Equal(1, strings.Length);
            Assert.Equal("single quote", strings[0]);
        }

        [Fact]
        public void TestCompability146()
        {
            var json = "abc";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string>(json));
        }

        // missing n_string_start_escape_unclosed

        [Fact]
        public void TestCompability147()
        {
            var json = "\"\"x";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string>(json));
        }

        [Fact]
        public void TestCompability148()
        {
            var json = "[[[[[[[[[[[[[[[[[[[[[[";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability149()
        {
            var json = "<.>";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability150()
        {
            var json = "[<null>]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability151()
        {
            var json = "[1]x";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability152()
        {
            var json = "[1]]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability153()
        {
            var json = "[\"abc]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability154()
        {
            var json = "[True]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability155()
        {
            var json = "]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability156()
        {
            var json = "{\"x\":true,";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, bool>>(json));
        }

        [Fact]
        public void TestCompability157()
        {
            var json = "[][]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability158()
        {
            var json = "[";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability159()
        {
            var json = "2@";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int>(json));
        }

        [Fact]
        public void TestCompability160()
        {
            var json = "{}}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability161()
        {
            var json = "{\"\":";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability162()
        {
            var json = "{\"a\":/*coment*/\"b\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability163()
        {
            var json = "{\"a\":true}\"x\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, bool>>(json));
        }

        [Fact]
        public void TestCompability164()
        {
            var json = "[,";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability165()
        {
            var json = "[{ \"\":[{\"\":[{\"\": ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>[]>(json));
        }

        [Fact]
        public void TestCompability167()
        {
            var json = "[{";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>[]>(json));
        }

        [Fact]
        public void TestCompability168()
        {
            var json = "[\"a";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability169()
        {
            var json = "[\"a\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability170()
        {
            var json = "{";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability171()
        {
            var json = "{,";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability172()
        {
            var json = "{]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability173()
        {
            var json = "{[";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability174()
        {
            var json = "\"\"";
            Assert.Equal(string.Empty, JsonMapper.To<string>(json));
        }

        [Fact]
        public void TestCompability175()
        {
            var json = "\"    \"";
            Assert.Equal("    ", JsonMapper.To<string>(json));
        }

        [Fact]
        public void TestCompability176()
        {
            var json = "{\"a\":\"\"}";
            var dict = JsonMapper.To<Dictionary<string, string>>(json);
            Assert.Equal(string.Empty, dict["a"]);
        }

        [Fact]
        public void TestCompability177()
        {
            var json = "[\"\"]";
            var array = JsonMapper.To<string[]>(json);
            Assert.Equal(string.Empty, array[0]);
        }

        [Fact]
        public void TestCompability178()
        {
            var json = "[\"]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability179()
        {
            var json = "{\"a\":\"}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability180()
        {
            var json = "{\"a\":\"  }";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability181()
        {
            var json = "[\"  ]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability182()
        {
            var json = "\"  ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability183()
        {
            var json = "[\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability184()
        {
            var json = "[\"  ";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability185()
        {
            var json = "{\"a";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability186()
        {
            var json = "{'a'";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<string[]>(json));
        }

        [Fact]
        public void TestCompability187()
        {
            var json = "{\"a\":\"b\"}#{}";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<Dictionary<string, string>>(json));
        }

        [Fact]
        public void TestCompability188()
        {
            var json = "[1";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<int[]>(json));
        }

        [Fact]
        public void TestCompability189()
        {
            var json = "[ false, null";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability190()
        {
            var json = "[ false, nul]";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability191()
        {
            var json = "[ false, tru";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability192()
        {
            var json = "[ true, fals";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability193()
        {
            var json = "{\"aaa\":\"bbb\"";
            Assert.Throws<ArgumentException>(() => JsonMapper.To<bool[]>(json));
        }

        [Fact]
        public void TestCompability194()
        {
            var json = "[[]     ]";
            var matrix = JsonMapper.To<int[][]>(json);
            Assert.Single(matrix);
            Assert.Empty(matrix[0]);
        }
        // y_array_arraysWithSpaces
    }
}
