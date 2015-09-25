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
	public struct AtomicInt32
	{
		private volatile Int32 value;

		private AtomicInt32(Int32 initialValue)
		{
			value = initialValue;
		}

		public static AtomicInt32 From(Int32 initialValue)
		{
			return new AtomicInt32(initialValue);
		}

		public Int32 Value
		{
			get
			{
				return value;
			}
		}

#pragma warning disable 420
		public bool CompareExchange(Int32 newValue, Int32 oldValue)
		{
			return Interlocked.CompareExchange(ref value, newValue, oldValue) == oldValue;
		}

		public bool Exchange(Int32 newValue)
		{
			var o = Interlocked.Exchange(ref value, newValue);
			return o == value;
		}
#pragma warning restore 420

		public override string ToString()
		{
			return value.ToString();
		}

		public static implicit operator int(AtomicInt32 atomic)
		{
			return atomic.Value;
		}
	
	}
}
