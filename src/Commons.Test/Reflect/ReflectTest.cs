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

namespace Commons.Test.Reflect
{
    public class ReflectTest
    {
		[Fact]
		public void TestReflectNewInstance1()
		{
			var vt = typeof(Vehicle);
			var vehicle = (Vehicle) vt.NewInstance();
			Assert.NotNull(vehicle);
		}

		[Fact]
		public void TestGetProperty1()
		{
			var v = new Vehicle
			{
				VehicleType = "Car",
				Color = "White"
			};

			var vt = (string)v.GetPropertyValue("VehicleType");
			Assert.Equal("Car", vt);
		}
    }
}
