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
using System.Threading;

namespace Commons.Utils
{
	[CLSCompliant(true)]
	public struct AtomicDouble
	{
		private Double value;

		private AtomicDouble(Double initialValue)
		{
			value = initialValue;
		}

		public static AtomicDouble From(Double initialValue)
		{
			return new AtomicDouble(initialValue);
		}

		public Double Value
		{
			get
			{
				return value;
			}
		}

		public bool CompareExchange(Double newValue, Double oldValue)
		{
			return Interlocked.CompareExchange(ref value, newValue, oldValue) == oldValue;
		}

		public bool Exchange(Double newValue)
		{
			var o = Interlocked.Exchange(ref value, newValue);
			return o == value;
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static implicit operator double(AtomicDouble atomic)
		{
			return atomic.Value;
		}
	
	}
}
