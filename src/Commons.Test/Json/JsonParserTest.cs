using System;
using Commons.Json.Mapper;
using Xunit;

namespace Commons.Test.Json
{
    public class JsonParserTest
    {
        [Fact]
        public void TestParseEngine01()
        {
            var json = "{\"name\": \"Jack\"}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = (JObject)value;
            Assert.Equal("Jack", (obj[new JString("name")]).Value);
            Assert.Equal("Jack", obj["name"].Value);
            Assert.Equal("Jack", obj["name"].Value);
        }

        [Fact]
        public void TestParseEngine02()
        {
            var json = "{\"nam\"e\": \"Jack\"}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine03()
        {
            var json = "{\"nam\"e\": \"Jack\"}{";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine04()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\"}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = value as JObject;
            Assert.Equal("usa", obj["country"].Value);
        }

        [Fact]
        public void TestParseEngine05()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = value as JObject;
            Assert.Equal(30, int.Parse(obj["age"].Value));
        }

        [Fact]
        public void TestParseEngine06()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30.}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine07()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": .30}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine08()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": -30}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = value as JObject;
            Assert.Equal(-30, int.Parse(obj["age"].Value));
        }

        [Fact]
        public void TestParseEngine09()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": -30.5}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = value as JObject;
            Assert.Equal(-30.5, double.Parse(obj["age"].Value));
        }

        [Fact]
        public void TestParseEngine10()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 3f0}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine11()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": false}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = value as JObject;
            Assert.True((obj["married"] as JBool) == JBool.False);
        }

        [Fact]
        public void TestParseEngine12()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": true}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json);
            Assert.True(value is JObject);
            var obj = value as JObject;
            Assert.True((obj["married"] as JBool) == JBool.True);
        }

        [Fact]
        public void TestParseEngine13()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": tRue}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine14()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": faLse}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine15()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": fase}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine16()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": \"fase}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
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
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine19()
        {
            var json = "{\"name\": \"Jack\", \"country\": \"usa\", \"age\": 30, \"married\": false, \"child\": Nill}";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine20()
        {
            var json = "\"Jason\"";
            var parseEngine = new JsonParseEngine();
            Assert.Equal("Jason", parseEngine.Parse(json).Value);
        }

        [Fact]
        public void TestParseEngine21()
        {
            var json = "20";
            var parseEngine = new JsonParseEngine();
            Assert.Equal("20", (parseEngine.Parse(json) as JNumber).Value);
        }

        [Fact]
        public void TestParseEngine22()
        {
            var json = "21.0002345";
            var parseEngine = new JsonParseEngine();
            Assert.Equal("21.0002345", (parseEngine.Parse(json) as JNumber).Value);
        }

        [Fact]
        public void TestParseEngine23()
        {
            var json = "{{ \"name\": \"Jack\"";
            var parseEngine = new JsonParseEngine();
            Assert.Throws(typeof(ArgumentException), () => parseEngine.Parse(json));
        }

        [Fact]
        public void TestParseEngine24()
        {
            var json = "[\"Jack\", \"John\", \"Jose\", \"Jame\"]";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json) as JArray;
            Assert.NotNull(value);
            Assert.Equal("Jack", value[0].Value);
            Assert.Equal("John", value[1].Value);
            Assert.Equal("Jose", value[2].Value);
            Assert.Equal("Jame", value[3].Value);
        }

        [Fact]
        public void TestParseEngine25()
        {
            var json = "[\"Jack\", [\"Sam\", \"Jon\"], \"Tom\"]";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json) as JArray;
            Assert.NotNull(value);
            Assert.Equal("Jack", value[0].Value);
            Assert.Equal("Sam", (value[1] as JArray)[0].Value);
            Assert.Equal("Jon", (value[1] as JArray)[1].Value);
            Assert.Equal("Tom", value[2].Value);
        }

        [Fact]
        public void TestParseEngine26()
        {
            var json = "{\"name\": \"Jack\", \"education\": { \"college\": \"nyu\", \"graduate\": \"mit\"}}";
            var parseEngine = new JsonParseEngine();
            var value = parseEngine.Parse(json) as JObject;
            Assert.NotNull(value);
            var edu = value["education"] as JObject;
            Assert.Equal("nyu", edu["college"].Value);
            Assert.Equal("mit", edu["graduate"].Value);
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
            Assert.Equal("27", (jack["age"] as JNumber).Value);
            Assert.Equal("usa", jack["country"].Value);
            //var jon = (value[1] as JObject)["Jon"];
            //Assert.NotNull(jon);
            //Assert.Equal("21", (jon["age"] as JPrimitive).Value);
            //Assert.Equal("canada", jon["country"] as JString);
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
            Assert.Equal("arsenal", englishTop[0].Value);
            Assert.Equal("man utd", englishTop[1].Value);
            Assert.Equal("chelsea", englishTop[2].Value);
            Assert.Equal("man city", englishTop[3].Value);
            Assert.Equal("chelsea", english["holder"].Value);
            var spain = value["spain"] as JObject;
            Assert.NotNull(spain);
            var spainTop = spain["top"] as JArray;
            Assert.NotNull(spainTop);
            Assert.Equal("barcelona", spainTop[0].Value);
            Assert.Equal("real", spainTop[1].Value);
            Assert.Equal("barcelona", spain["holder"].Value);
        }

        [Fact]
        public void TestParseEngine29()
        {
            var engine = new JsonParseEngine();
            var json4 = TestHelper.ReadFrom("JsonSample4.txt");
            engine.Parse(json4);
            var json5 = TestHelper.ReadFrom("JsonSample5.txt");
            new JsonParseEngine().Parse(json5);
            var json6 = TestHelper.ReadFrom("JsonSample6.txt");
            new JsonParseEngine().Parse(json6);
            var json7 = TestHelper.ReadFrom("JsonSample7.txt");
            new JsonParseEngine().Parse(json7);
            var json8 = TestHelper.ReadFrom("JsonSample8.txt");
            new JsonParseEngine().Parse(json8);
            var json9 = TestHelper.ReadFrom("JsonSample10.txt");
            new JsonParseEngine().Parse(json9);
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
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
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
			var value = engine.Parse(json);
			Assert.True(value is JArray);
			Assert.Equal("a", (value as JArray)[0].Value);
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
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine41()
        {
            var engine = new JsonParseEngine();
            var json = "[[[[[[null]]]]]]]]]]";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine42()
        {
            var engine = new JsonParseEngine();
            var json = "[{}, {},{}}]";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine43()
        {
            var engine = new JsonParseEngine();
            var json = ",";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine44()
        {
            var engine = new JsonParseEngine();
            var json = "{,\"a\" : \"b\",}";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine45()
        {
            var engine = new JsonParseEngine();
            var json = "[,\"a\", ]";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }

        [Fact]
        public void TestParseEngine46()
        {
            var engine = new JsonParseEngine();
            var json = "{\"a\": \"b\", \"c\"}";
            Assert.Throws(typeof(ArgumentException), () => engine.Parse(json));
        }
    }
}
