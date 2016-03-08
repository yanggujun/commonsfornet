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

using System.Collections.Generic;
namespace Test.Commons.Json
{
	public class Simple
	{
		public string FieldA { get; set; }

        public int FieldB { get; set; }

        public double FieldC { get; set; }

        public bool FieldD { get; set; }
	}

	public class Nested
	{
		public string FieldE { get; set; }

		public Simple Simple { get; set; }

		public int FieldF { get; set; }

		public double FieldG { get; set; }

		public bool FieldH { get; set; }
	}

    public class ArrayNested
    {
        public string FieldI { get; set; }

        public List<Nested> NestedItems { get; set; }
    }
}
