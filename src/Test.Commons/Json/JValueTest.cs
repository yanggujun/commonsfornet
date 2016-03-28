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
using Commons.Json;
using Commons.Json.Mapper;
using Xunit;
namespace Test.Commons.Json
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
    }
}
