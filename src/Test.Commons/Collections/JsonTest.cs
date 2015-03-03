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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.Json;
using Xunit;

namespace Test.Commons.Collections
{
    public class JsonTest
    {
        [Fact]
        public void TestJsonObject()
        {
            dynamic worldCup = new JsonObject();
            worldCup.Host = "Brazil";
            worldCup.TotalTeams = 32;
            worldCup.Started = true;
            worldCup.Groups.GroupA.Teams = new[] { "Brazil", "Crotia", "Mexico", "Cameroon" };
            worldCup.Groups.GroupA.Results.Day1 = new[] { "Bra 3 : 1 Cro", "Mex 2 : 1 Cameroon" };
            worldCup.Groups.GroupA.Results.Day2 = new[] { "Bra 0 : 0 Mex", "Cro 4 : 0 Cameroon" };
            worldCup.Groups.GroupA.Qualifiers.First = "Brazil";
            worldCup.Groups.GroupA.Qualifiers.Second = "Mexico";
            worldCup.Groups.GroupB = new[] { "Spain", "Netherland", "Chile", "Austrilia" };
            worldCup.Groups.GroupC = new[] { "Columbia", "Greece", "Cote Divoire", "Japan" };
            worldCup.Cities = new[] { "Rio", "Brazilia", "San Paulo", "Salvador" };
            worldCup.Groups.GroupA.TimeStart = new DateTime(2014, 6, 17);
            Assert.Equal("Spain", (string)worldCup.Groups.GroupB[0]);
            Assert.Equal("Netherland", (string)worldCup.Groups.GroupB[1]);
            Assert.Equal("Chile", (string)worldCup.Groups.GroupB[2]);
            Assert.Equal("Austrilia", (string)worldCup.Groups.GroupB[3]);
            Assert.DoesNotThrow(() => JsonObject.Parse(worldCup.ToString()));
        }

        [Fact]
        public void TestParseJson()
        {
            string json = TestHelper.ReadFrom(@".\Collections\JsonSample.txt");
            dynamic worldcup = JsonObject.Parse(json);
            string host = worldcup.Host;
            int teams = worldcup.Teams;
            Assert.Equal("Brazil", host);
            Assert.Equal(32, teams);
            Assert.True((bool)worldcup.Started);
            string champ = worldcup.Champion;
            Assert.Equal(null, champ);
        }

        [Fact]
        public void TestParseJson2()
        {
            var json = TestHelper.ReadFrom(@".\Collections\JsonSample2.txt");
            dynamic package = JsonObject.Parse(json);
            Assert.Equal("yanggujun", (string)package.author);
            Assert.Equal("commons", (string)package.name);
            Assert.Equal("git", (string)package.repository.type);
            Assert.Equal("~0.10.2", (string)package.devDependencies.karma);
            Assert.Equal(null, (string)package.bugs.url);
        }

        [Fact]
        public void TestParseJson3()
        {
            var json = TestHelper.ReadFrom(@".\Collections\JsonSample3.txt");
            dynamic package = JsonObject.Parse(json);
            Assert.Equal("yanggujun", (string)package.author);
            Assert.Equal("no logo", (string)package.logo);
            Assert.Equal("~1.8", (string)package.devDependencies.chai);
        }

        public void TestParseJsonIllEmptyHalfObject()
        {
            var json = "{";
            Assert.Throws(typeof(ArgumentException), () => JsonObject.Parse(json));
        }

        [Fact]
        public void TestParseJsonEmptyObject()
        {
            var json = @"{}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(string.Empty, jobj.ToString().Trim('{', '}').Trim());
        }

        [Fact]
        public void TestParseJsonNestedEmptyObject()
        {
            var json = "{\"test\": {}}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(string.Empty, jobj.test.ToString().Trim('{', '}').Trim());
        }

        [Fact]
        public void TestParseJsonNestedEmptyArrayObject()
        {
            var json = "{\"test\": [{}]}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(string.Empty, jobj.test[0].ToString().Trim('{', '}').Trim());
        }

        [Fact]
        public void TestParseJsonEmptyArray()
        {
            var json = "{\"test\": []}";
            dynamic jobj = JsonObject.Parse(json);
            foreach (string item in jobj.test)
            {
                Assert.Equal(0, 1);
            }
        }

        [Fact]
        public void TestParseEmptyValue()
        {
            var json = "{\"test\": }";
            Assert.Throws(typeof(ArgumentException), () => JsonObject.Parse(json));
        }

        [Fact]
        public void TestParseJsonNonEmptyArray()
        {
            var json = "{\"test\": [\"as\"]}";
            dynamic jobj = JsonObject.Parse(json);
            foreach (var item in jobj.test)
            {
                Assert.Equal("as", (string)item);
            }
        }

        [Fact]
        public void TestParseJsonNestedNullValue()
        {
            var json = "{\"test\": { \"nest\": null}}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(null, (string)jobj.test.nest);
        }

        [Fact]
        public void TestParseJsonNestedEmptyValue()
        {
            var json = "{\"test\": { \"nest\": }}";
            Assert.Throws(typeof(ArgumentException), () => JsonObject.Parse(json));
        }

        [Fact]
        public void TestParseJsonIllFormat()
        {
            var json = "{\"test\": { \"nest\": null}";
            Assert.Throws(typeof(ArgumentException), () => JsonObject.Parse(json));
        }

        [Fact]
        public void TestParseJsonNumberArray()
        {
            var json = "{\"test\": { \"nest\": [1, 2, 3, 4, 5, 6]}}";
            dynamic jobj = JsonObject.Parse(json);
            var count = 1;
            foreach (var item in jobj.test.nest)
            {
                Assert.Equal(count++, (int)item);
            }
        }

        [Fact]
        public void TestParseJsonNullArray()
        {
            var json = "{\"test\": { \"nest\": [null, null]}}";
            dynamic jobj = JsonObject.Parse(json);
            foreach (var item in jobj.test.nest)
            {
                Assert.Equal(null, (string)item);
            }
        }

        [Fact]
        public void TestParseJsonMixedNumberArray()
        {
            var json = "{\"test\": { \"nest\": [1, 1.1, 2, 3.5, 4.0]}}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(1, (int)jobj.test.nest[0]);
            Assert.Equal(1.1, (double)jobj.test.nest[1], 2);
            Assert.Equal(2, (int)jobj.test.nest[2]);
            Assert.Equal(3.5, (double)jobj.test.nest[3], 2);
            Assert.Equal(4.0, (double)jobj.test.nest[4], 2);
        }

        [Fact]
        public void TestParseJsonObjectArray()
        {
            var json = "{\"test\": { \"nest\": [{\"a\": 1, \"b\": 2}, {\"c\": 3}, 5, {}, null]}}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(1, (int)jobj.test.nest[0].a);
            Assert.Equal(2, (int)jobj.test.nest[0].b);
            Assert.Equal(3, (int)jobj.test.nest[1].c);
            Assert.Equal(5, (int)jobj.test.nest[2]);
            Assert.Equal(string.Empty, jobj.test.nest[3].ToString().Trim('{', '}').Trim());
            Assert.Equal(null, (string)jobj.test.nest[4]);
        }

        [Fact]
        public void TestParseJsonIllEmptyObject()
        {
            var json = "{\"test\": { \"nest\": {null}}}";
            Assert.Throws(typeof(ArgumentException), () => JsonObject.Parse(json));
        }

        [Fact]
        public void TestParseJsonSetSimpleValue()
        {
            var json = "{\"test\": { \"nest\": 1}}";
            dynamic jobj = JsonObject.Parse(json);
            Assert.Equal(1, (int)jobj.test.nest);
            jobj.test.nest = "newstring";
            var newJson = (string)jobj;
            dynamic newObj = JsonObject.Parse(newJson);
            Assert.Equal("newstring", (string)newObj.test.nest);
        }

        [Fact]
        public void TestParseJsonSetArrayValue()
        {
            var json = "{\"test\": { \"nest\": [1, null]}}";
            dynamic jobj = JsonObject.Parse(json);
            jobj.test.nest[1] = 2;
            var newJson = (string)jobj;
            dynamic newObj = JsonObject.Parse(newJson);
            Assert.Equal(2, (int)newObj.test.nest[1]);
        }

        [Fact]
        public void TestParseJsonStrings()
        {
            var json4 = TestHelper.ReadFrom(@".\Collections\JsonSample4.txt");
            Assert.DoesNotThrow(() => JsonObject.Parse(json4));
            var json5 = TestHelper.ReadFrom(@".\Collections\JsonSample5.txt");
            Assert.DoesNotThrow(() => JsonObject.Parse(json5));
            var json6 = TestHelper.ReadFrom(@".\Collections\JsonSample6.txt");
            Assert.DoesNotThrow(() => JsonObject.Parse(json6));
            var json7 = TestHelper.ReadFrom(@".\Collections\JsonSample7.txt");
            Assert.DoesNotThrow(() => JsonObject.Parse(json7));
            var json8 = TestHelper.ReadFrom(@".\Collections\JsonSample8.txt");
            Assert.DoesNotThrow(() => JsonObject.Parse(json8));
        }
    }
}
