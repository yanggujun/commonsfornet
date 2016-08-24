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
using System.ComponentModel;
using Commons.Collections.Set;
using Commons.Json;
using Commons.Json.Mapper;
using Xunit;
namespace Commons.Test.Json
{
    public class JValueTest
    {
        [Fact]
        public void TestJObject01()
        {
            var employee = new JObject();
            employee.SetString("Name", "Alan");
            employee.SetString("Site", "London");
            employee.SetString("department", "hardware");
            employee.SetInt32("age", 40);
            employee.SetString("birthday", new DateTime(1975, 10, 2).ToString());
            employee.SetBool("married", true);
            employee.SetString("position", "manager");
            var json = employee.ToString();
            JsonMapper.UseDateFormat(string.Empty).For<Employee>().MapProperty(x => x.Dob).With("birthday");
            var emp = JsonMapper.To<Employee>(json);
            Assert.Equal("Alan", emp.Name);
            Assert.Equal(Site.London, emp.Site);
            Assert.Equal("hardware", emp.Department);
            Assert.Equal(40, emp.Age);
            Assert.True(emp.Married);
            Assert.Equal("manager", emp.Position);
            Assert.Equal(1975, emp.Dob.Year);
            Assert.Equal(10, emp.Dob.Month);
            Assert.Equal(2, emp.Dob.Day);
        }

        [Fact]
        public void TestJObject02()
        {
            var employee = new Employee
            {
                Name = "Alan",
                Department = "hardware",
                Married = true,
                Position = "manager",
                Dob = new DateTime(1975, 10, 2),
                Site = Site.London,
                Age = 40
            };
            JsonMapper.UseDateFormat(string.Empty).For<Employee>().MapProperty(x => x.Dob).With("birthday");
            var json = JsonMapper.ToJson(employee);
            var engine = new JsonParseEngine();
            var emp = engine.Parse(json) as JObject;
            Assert.NotNull(emp);
            Assert.Equal("Alan", emp.GetString("name"));
            Assert.Equal("hardware", emp.GetString("Department"));
            Assert.Equal("manager", emp.GetString("POSITION"));
            Assert.Equal(40, emp.GetInt32("age"));
            Assert.True(emp.GetBool("married"));
            var dt = DateTime.Parse(emp.GetString("birthday"));
            Assert.Equal(1975, dt.Year);
            Assert.Equal(10, dt.Month);
            Assert.Equal(2, dt.Day);
        }

        [Fact]
        public void TestJObject03()
        {
            var company = new JObject();
            company.SetString("name", "OneTech");
            company.SetString("country", "USA");
            company.SetInt16("Staff Count", 20000);
            company.SetInt64("Revenue", 20000000000);
            var employees = new JArray();
            var alan = new JObject();
            alan.SetString("name", "Alan");
            alan.SetString("Department", "hardware");
            alan.SetString("position", "manager");
            alan.SetBool("Married", true);
            alan.SetInt32("age", 40);
            alan.SetString("birthday", new DateTime(1975, 10, 2).ToString());
            alan.SetEnum("site", Site.Paris);
            var bran = new JObject();
            bran.SetString("name", "Bran");
            bran.SetString("department", "software");
            bran.SetString("position", "engineer");
            bran.SetBool("married", false);
            bran.SetInt32("age", 25);
            bran.SetString("birthday", new DateTime(1990, 2, 24).ToString());
            bran.SetEnum("site", Site.NY);
            employees.Add(alan);
            employees.Add(bran);
            company.SetArray("Employees", employees);
            var json = company.ToString();
            JsonMapper.UseDateFormat(string.Empty).For<Employee>().MapProperty(x => x.Dob).With("birthday");
            JsonMapper.For<Company>().MapProperty(x => x.StaffCount).With("Staff Count");
            var comp = JsonMapper.To<Company>(json);
            Assert.Equal("OneTech", comp.Name);
            Assert.Equal("USA", comp.Country);
            Assert.Equal(20000, comp.StaffCount);
            Assert.Equal(20000000000, comp.Revenue);
            Assert.Equal(2, comp.Employees.Count);

            Assert.Equal("Alan", comp.Employees[0].Name);
            Assert.Equal("hardware", comp.Employees[0].Department);
            Assert.Equal("manager", comp.Employees[0].Position);
            Assert.Equal(40, comp.Employees[0].Age);
            Assert.True(comp.Employees[0].Married);
            Assert.Equal(1975, comp.Employees[0].Dob.Year);
            Assert.Equal(10, comp.Employees[0].Dob.Month);
            Assert.Equal(2, comp.Employees[0].Dob.Day);
            Assert.Equal(Site.Paris, comp.Employees[0].Site);

            Assert.Equal("Bran", comp.Employees[1].Name);
            Assert.Equal("software", comp.Employees[1].Department);
            Assert.Equal("engineer", comp.Employees[1].Position);
            Assert.Equal(25, comp.Employees[1].Age);
            Assert.False(comp.Employees[1].Married);
            Assert.Equal(1990, comp.Employees[1].Dob.Year);
            Assert.Equal(2, comp.Employees[1].Dob.Month);
            Assert.Equal(24, comp.Employees[1].Dob.Day);
            Assert.Equal(Site.NY, comp.Employees[1].Site);
        }

        [Fact]
        public void TestJObject04()
        {
            var obj = new JObject();
            obj.SetByte("a1", 2);
            obj.SetSByte("a2", -2);
            obj.SetInt16("a3", -3);
            obj.SetUInt16("a4", 3);
            obj.SetInt32("a5", -4);
            obj.SetUInt32("a6", 4);
            obj.SetInt64("a7", 9);
            obj.SetDouble("a8", 4.33356);
            obj.SetSingle("a9", 4.3f);
            obj.SetDecimal("a10", 10.2312123m);
            obj.SetBool("a11", false);
            obj.SetEnum("a12", Site.SH);
            obj.SetString("a13", "value12");
            var array = new JArray();
            for (var i = 0; i < 10; i++)
            {
                array.AddInt32(i);
            }
            obj.SetArray("a14", array);
            var json = obj.ToString();

            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal(2, (byte)jsonObj.a1);
            Assert.Equal(-2, (sbyte)jsonObj.a2);
            Assert.Equal(-3, (short)jsonObj.a3);
            Assert.Equal(3, (ushort)jsonObj.a4);
            Assert.Equal(-4, (int)jsonObj.a5);
            Assert.Equal((uint)4, (uint)jsonObj.a6);
            Assert.Equal(9, (long)jsonObj.a7);
            Assert.Equal(4.33356, (double)jsonObj.a8);
            Assert.Equal(4.3f, (float)jsonObj.a9);
            Assert.Equal(10.2312123m, (decimal)jsonObj.a10);
            Assert.False((bool)jsonObj.a11);
            Assert.Equal("SH", (string)jsonObj.a12);
            Assert.Equal("value12", (string)jsonObj.a13);
            for (var i = 0; i < 10; i++)
            {
                Assert.Equal(i, (short)jsonObj.a14[i]);
            }
        }

        [Fact]
        public void TestJObject05()
        {
            var obj = new JObject();
            obj["key1"] = JNull.Value;
            obj.SetBool("key2", false);

            var json = obj.ToString();
            var jsonObj = JsonMapper.Parse(json);
            Assert.Null(jsonObj.key1);
            Assert.False((bool)jsonObj.key2);
        }

        [Fact]
        public void TestJArray01()
        {
            var json = "[0, 1, 2, 3, 4, 5, 6, \"value1\", true, 10.5386, 2.12453, 4.3]";
            var engine = new JsonParseEngine();
            var array = engine.Parse(json) as JArray;
            var a0 = array[0] as JInteger;
            Assert.NotNull(a0);
            Assert.Equal(0, a0.AsByte());
            var a1 = array[1] as JInteger;
            Assert.NotNull(a1);
            Assert.Equal(1, a1.AsSByte());
            var a2 = array[2] as JInteger;
            Assert.NotNull(a2);
            Assert.Equal(2, a2.AsInt16());
            var a3 = array[3] as JInteger;
            Assert.NotNull(a3);
            Assert.Equal(3, a3.AsUInt16());
            var a4 = array[4] as JInteger;
            Assert.NotNull(a4);
            Assert.Equal(4, a4.AsInt32());
            var a5 = array[5] as JInteger;
            Assert.NotNull(a5);
            Assert.Equal((uint)5, a5.AsUInt32());
            var a6 = array[6] as JInteger;
            Assert.NotNull(a6);
            Assert.Equal(6, a6.AsInt64());
            Assert.Equal("value1", array[7] as JString);
            Assert.True(array[8] as JBoolean);
            var a9 = array[9] as JDecimal;
            Assert.NotNull(a9);
            Assert.Equal(10.5386, a9.AsDouble());
            var a10 = array[10] as JDecimal;
            Assert.NotNull(a10);
            Assert.Equal(2.12453m, a10.AsDecimal());
            var a11 = array[11] as JDecimal;
            Assert.NotNull(a11);
            Assert.Equal(4.3f, a11.AsSingle());
        }

        [Fact]
        public void TestJArray02()
        {
            var json = "{\"key1\": [ 0, 1, 2, 3, 4, 5, 6], \"key2\": null}";
            var engine = new JsonParseEngine();
            var obj = engine.Parse(json) as JObject;
            Assert.NotNull(obj);
            var array = obj.GetArray("key1");
            for (var i = 0; i < 7; i++)
            {
                var integer = array[i] as JInteger;
                Assert.Equal(i, integer.AsInt32());
            }
            var jnull = obj["key2"] as JNull;
            Assert.NotNull(jnull);
            Assert.True(ReferenceEquals(jnull, JNull.Value));
        }

        [Fact]
        public void TestJArray03()
        {
            var json = "[\"value1\", \"value2\", \"value3\", \"value4\", true, 4, 2.5]";
            var engine = new JsonParseEngine();
            var array = engine.Parse(json) as JArray;
            Assert.NotNull(array);
            var str = "value";
            for (var i = 0; i < 4; i++)
            {
                Assert.Equal(str + (i + 1), (JString)array[i]);
                
            }
            Assert.True((JBoolean)array[4]);
            Assert.Equal(4, ((JInteger)array[5]).AsInt32());
            Assert.Equal(2.5, ((JDecimal)array[6]).AsDouble());
        }

        [Fact]
        public void TestJString()
        {
            var str1 = new JString("jsonstring");
            var str2 = new JString("jsonstring");

            Assert.True(str1.Equals(str2));
            var set = new HashedSet<JString>();
            set.Add(str1);
            Assert.True(set.Contains(str2));
        }

        [Fact]
        public void TestJInteger()
        {
            var integer1 = new JInteger(1000);
            var integer2 = new JInteger(1000);
            Assert.True(integer1.Equals(integer2));
            var set = new HashedSet<JInteger>();
            set.Add(integer1);
            Assert.True(set.Contains(integer2));
        }

        [Fact]
        public void TestJDecimal()
        {
            var dec1 = new JDecimal(10.45976m);
            var dec2 = new JDecimal(10.45976m);
            Assert.True(dec1.Equals(dec2));
        }

        [Fact]
        public void TestJBoolean()
        {
            var b1 = new JBoolean(false);
            var b2 = new JBoolean(false);
            Assert.True(b1.Equals(b2));
        }

        [Fact]
        public void TestJsonConverter01()
        {
            var json =
                "{\"Name\": \"Ben\", \"Position\": \"Director\", \"Married\": True, \"Dob\": \"1970/4/3\", \"Department\": \"Sales\", \"Age\": 46, \"Site\": \"NY\"}";
            var employee = JsonMapper.To<Employee>(json, new EmployeeJsonConverter());
            Assert.Equal(46, employee.Age);
            Assert.Equal("Sales", employee.Department);
            Assert.Equal(1970, employee.Dob.Year);
            Assert.Equal(4, employee.Dob.Month);
            Assert.Equal(3, employee.Dob.Day);
            Assert.Equal("Ben", employee.Name);
            Assert.Equal("Director", employee.Position);
            Assert.True(employee.Married);
        }

        [Fact]
        public void TestJsonConverter02()
        {
            var jsonStr =
                "{\"Name\": \"Ben\", \"Position\": \"Director\", \"Married\": True, \"Dob\": \"1970/4/3\", \"Department\": \"Sales\", \"Age\": 46, \"Site\": \"NY\"}";
            var employee = JsonMapper.To<Employee>(jsonStr, x =>
            {
                var json = x as JObject;
                var emp = new Employee();
                emp.Age = json.GetInt32("age");
                emp.Department = json.GetString("Department");
                emp.Dob = DateTime.Parse(json.GetString("dob"));
                emp.Married = json.GetBool("Married");
                emp.Name = json.GetString("name");
                emp.Position = json.GetString("Position");
                emp.Site = json.GetEnum<Site>("site");
                return emp;
            });
            Assert.Equal(46, employee.Age);
            Assert.Equal("Sales", employee.Department);
            Assert.Equal(1970, employee.Dob.Year);
            Assert.Equal(4, employee.Dob.Month);
            Assert.Equal(3, employee.Dob.Day);
            Assert.Equal("Ben", employee.Name);
            Assert.Equal("Director", employee.Position);
            Assert.True(employee.Married);
        }

        [Fact]
        public void TestJsonConverter03()
        {
            var json = "[\"value1\", \"value2\", \"value3\", \"value4\"]";
            var collection = JsonMapper.To(json, value =>
            {
                var set = new HashedSet<string>();
                var array = value as JArray;
                foreach (var item in array)
                {
                    set.Add(item as JString);
                }
                return set;
            });

            collection.Contains("value1");
            collection.Contains("value2");
            collection.Contains("value3");
            collection.Contains("value4");
        }

        [Fact]
        public void TestJsonConverter04()
        {
            var json1 = "{\"ourList\": {\"name\": \"X\", \"value\": \"Y\"}}";
            var json2= "{\"ourList\": [{\"name\": \"X1\", \"value\": \"Y1\"}, {\"name\": \"X2\", \"value\": \"Y2\"}]}";
            var l1 = JsonMapper.To<OuterList>(json1, Convert);
            Assert.Equal(1, l1.OurList.Length);
            Assert.Equal("X", l1.OurList[0].Name);
            Assert.Equal("Y", l1.OurList[0].Value);

            var l2 = JsonMapper.To<OuterList>(json2, Convert);
            Assert.Equal(2, l2.OurList.Length);
            Assert.Equal("X1", l2.OurList[0].Name);
            Assert.Equal("Y1", l2.OurList[0].Value);
            Assert.Equal("X2", l2.OurList[1].Name);
            Assert.Equal("Y2", l2.OurList[1].Value);
        }

        private OuterList Convert(JValue value)
        {
            var jsonObj = value as JObject;
            var outer = new OuterList();
            if (jsonObj["ourList"] is JArray)
            {
                var array = jsonObj["OurList"] as JArray;
                outer.OurList = new Member[array.Length];
                for (var i = 0; i < array.Length; i++)
                {
                    outer.OurList[i] = new Member
                    {
                        Name = (array[i] as JObject).GetString("name"),
                        Value = (array[i] as JObject).GetString("value")
                    };
                }
            }
            else
            {
                outer.OurList = new Member[1];
                outer.OurList[0] = new Member
                {
                    Name = (jsonObj["ourList"] as JObject).GetString("name"),
                    Value = (jsonObj["ourList"] as JObject).GetString("value")
                };
            }

            return outer;
        }

        [Fact]
        public void TestObjectConverter01()
        {
            var employee = new Employee
            {
                Age = 46,
                Department = "Sales",
                Dob = new DateTime(1970, 4, 3),
                Married = true,
                Name = "Ben",
                Position = "Director",
                Site = Site.NY
            };
            var json = JsonMapper.ToJson(employee, new EmployeeObjectConverter());
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Ben", (string) jsonObj.Name);
            Assert.Equal("Sales", (string) jsonObj.Department);
            Assert.Equal("Director", (string) jsonObj.Position);
            Assert.Equal(46, (int) jsonObj.Age);
            Assert.Equal("NY", (string)jsonObj.Site);
            Assert.True((bool)jsonObj.Married);
            var dob = DateTime.Parse((string) jsonObj.Dob);
            Assert.Equal(1970, dob.Year);
            Assert.Equal(4, dob.Month);
            Assert.Equal(3, dob.Day);
        }

        [Fact]
        public void TestObjectConverter02()
        {
            var employee = new Employee
            {
                Age = 46,
                Department = "Sales",
                Dob = new DateTime(1970, 4, 3),
                Married = true,
                Name = "Ben",
                Position = "Director",
                Site = Site.NY
            };
            var json = JsonMapper.ToJson(employee, emp =>
            {
                var obj = new JObject();
                obj.SetString("Name", emp.Name);
                obj.SetString("Position", emp.Position);
                obj.SetString("Department", emp.Department);
                obj.SetString("Site", emp.Site.ToString());
                obj.SetInt32("Age", emp.Age);
                obj.SetBool("Married", emp.Married);
                obj.SetString("Dob",emp.Dob.ToString());
                return obj;
            });
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Ben", (string) jsonObj.Name);
            Assert.Equal("Sales", (string) jsonObj.Department);
            Assert.Equal("Director", (string) jsonObj.Position);
            Assert.Equal(46, (int) jsonObj.Age);
            Assert.Equal("NY", (string)jsonObj.Site);
            Assert.True((bool)jsonObj.Married);
            var dob = DateTime.Parse((string) jsonObj.Dob);
            Assert.Equal(1970, dob.Year);
            Assert.Equal(4, dob.Month);
            Assert.Equal(3, dob.Day);
        }

        [Fact]
        public void TestObjectSerializer01()
        {
            JsonMapper.For<Member>().SerializeBy(x =>
            {
                var jo = new JObject();
                jo.SetString("member name", x.Name);
                jo.SetString("member value", x.Value);
                return jo;
            });

            var c = new ComplexMember
            {
                F1 = "v1",
                F2 = "v2",
                F3 = "v3",
                Member = new Member
                {
                    Name = "Location",
                    Value = "Chicago"
                }
            };

            var json = JsonMapper.ToJson(c);

            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Location", (string)jsonObj.Member["member name"]);
            Assert.Equal("Chicago", (string)jsonObj.Member["member value"]);
            Assert.Equal("v1", (string)jsonObj.F1);
            Assert.Equal("v2", (string)jsonObj.F2);
            Assert.Equal("v3", (string)jsonObj.F3);
        }

        [Fact]
        public void TestObjectSerializer02()
        {
            JsonMapper.For<Member>().SerializeBy(x =>
            {
                var jo = new JObject();
                jo.SetString("member name", x.Name);
                jo.SetString("member value", x.Value);
                return jo;
            });

            var m = new Member { Name = "Club", Value = "Arsenal" };

            var json = JsonMapper.ToJson(m);
            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("Club", (string)jsonObj["member name"]);
            Assert.Equal("Arsenal", (string)jsonObj["member value"]);
        }

        [Fact]
        public void TestObjectSerializer03()
        {
            JsonMapper.For<Class1>().SerializeBy(new Class1Converter());

            var c = new Class2
            {
                F3 = "value33333",
                Items = new Class1[]
                {
                    new Class1 { F1 = "value1", F2 = "value2" },
                    new Class1 { F1 = "value3", F2 = "value4" }
                }
            };

            var json = JsonMapper.ToJson(c);

            var jsonObj = JsonMapper.Parse(json);
            Assert.Equal("value33333", (string)jsonObj.F3);
            Assert.Equal("value1", (string)jsonObj.Items[0].FieldA);
            Assert.Equal("value2", (string)jsonObj.Items[0].FieldB);
            Assert.Equal("value3", (string)jsonObj.Items[1].FieldA);
            Assert.Equal("value4", (string)jsonObj.Items[1].FieldB);
        }
    }

    public class EmployeeObjectConverter : IObjectConverter<Employee>
    {
        public JValue Convert(Employee target)
        {
            var obj = new JObject();
            obj.SetString("Name", target.Name);
            obj.SetString("Position", target.Position);
            obj.SetString("Department", target.Department);
            obj.SetString("Site", target.Site.ToString());
            obj.SetInt32("Age", target.Age);
            obj.SetBool("Married", target.Married);
            obj.SetString("Dob", target.Dob.ToString());
            return obj;
        }
    }

    public class EmployeeJsonConverter : IJsonConverter<Employee>
    {
        public Employee Convert(JValue jsonValue)
        {
            var json = jsonValue as JObject;
            var employee = new Employee();
            employee.Age = json.GetInt32("age");
            employee.Department = json.GetString("Department");
            employee.Dob = DateTime.Parse(json.GetString("dob"));
            employee.Married = json.GetBool("Married");
            employee.Name = json.GetString("name");
            employee.Position = json.GetString("Position");
            employee.Site = json.GetEnum<Site>("site");
            return employee;
        }
    }

    public class Member
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class OuterList
    {
        public Member[] OurList { get; set; }
    }

    public class ComplexMember
    {
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }

        public Member Member { get; set; }
    }

    public class Class1
    {
        public string F1 { get; set; }
        public string F2 { get; set; }
    }

    public class Class2
    {
        public string F3 { get; set; }
        public Class1[] Items { get; set; }
    }

    public class Class1Converter : IObjectConverter<Class1>
    {
        public JValue Convert(Class1 target)
        {
            var jsonObj = new JObject();
            jsonObj.SetString("FieldA", target.F1);
            jsonObj.SetString("FieldB", target.F2);
            return jsonObj;
        }
    }
}
