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
    }
}
