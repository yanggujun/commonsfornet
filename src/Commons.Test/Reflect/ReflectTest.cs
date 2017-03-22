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

using Xunit;

using Commons.Reflect;
using Commons.Test.Poco;

namespace Commons.Test.Reflect
{
    public class ReflectTest
    {
		[Fact]
		public void TestReflectNewInstance1()
		{
			var simpleType = typeof(Simple);
            var invoker = Reflector.GetInvoker(simpleType);
            var simple = (Simple)invoker.NewInstance();
			Assert.NotNull(simple);
		}

		[Fact]
		public void TestGetProperty1()
		{
			var simple = new Simple
			{
				FieldA = "ValueA",
				FieldB = 1912,
				FieldC = 1412.2,
				FieldD = true
			};

            var invoker = Reflector.GetInvoker(typeof(Simple));
            var fa = (string)invoker.GetProperty(simple, "FieldA");
			Assert.Equal(simple.FieldA, fa);
			var fb = (int)invoker.GetProperty(simple, "FieldB");
			Assert.Equal(simple.FieldB, fb);
			var fc = (double)invoker.GetProperty(simple, "FieldC");
			Assert.Equal(simple.FieldC, fc);
			var fd = (bool)invoker.GetProperty(simple, "FieldD");
			Assert.Equal(simple.FieldD, fd);
		}

        [Fact]
        public void TestSetProperty1()
        {
			var simple = new Simple();
            var invoker = Reflector.GetInvoker(typeof(Simple));
			invoker.SetProperty(simple, "FieldA", "ValueA");
			invoker.SetProperty(simple, "FieldB", 10);
			invoker.SetProperty(simple, "FieldC", 20.45);
			invoker.SetProperty(simple, "FieldD", false);
			Assert.Equal("ValueA", simple.FieldA);
			Assert.Equal(10, simple.FieldB);
			Assert.Equal(20.45, simple.FieldC);
			Assert.Equal(false, simple.FieldD);
        }
    }
}
