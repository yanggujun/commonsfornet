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
    public struct AtomicInt64
    {
        private Int64 value;

        private AtomicInt64(Int64 initialValue)
        {
            value = initialValue;
        }

        public static AtomicInt64 From(Int64 initialValue)
        {
            return new AtomicInt64(initialValue);
        }

        public Int64 Value
        {
            get
            {
#if NET45
                return Volatile.Read(ref value);
#endif
#if NET40
                return Thread.VolatileRead(ref value);
#endif 
            }
        }

        public bool CompareExchange(Int64 newValue, Int64 oldValue)
        {
            return Interlocked.CompareExchange(ref value, newValue, oldValue) == oldValue;
        }

        public bool Exchange(Int64 newValue)
        {
            var o = value;
            var r = Interlocked.Exchange(ref value, newValue);
            return o == r;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator long(AtomicInt64 atomic)
        {
            return atomic.Value;
        }

        public void Increment()
        {
            Interlocked.Increment(ref value);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref value);
        }

	    public static bool operator <(AtomicInt64 left, AtomicInt64 right)
	    {
		    var leftValue = left.Value;
		    var rightValue = right.Value;
		    return leftValue < rightValue;
	    }

	    public static bool operator >=(AtomicInt64 left, AtomicInt64 right)
	    {
		    var leftValue = left.value;
		    var rightValue = right.Value;
		    return leftValue >= rightValue;
	    }

	    public static bool operator <=(AtomicInt64 left, AtomicInt64 right)
	    {
		    var leftValue = left.Value;
		    var rightValue = right.Value;
		    return leftValue <= rightValue;
	    }

	    public static bool operator >(AtomicInt64 left, AtomicInt64 right)
	    {
		    var leftValue = left.Value;
		    var rightValue = right.Value;
		    return leftValue > rightValue;
	    }

        public static AtomicInt64 operator +(AtomicInt64 left, AtomicInt64 right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;
            return AtomicInt64.From(leftValue + rightValue);
        }

        public static AtomicInt64 operator +(AtomicInt64 left, Int32 right)
        {
            Interlocked.Add(ref left.value, right);
            return left;
        }

        public static AtomicInt64 operator -(AtomicInt64 left, AtomicInt64 right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;
            return AtomicInt64.From(leftValue - rightValue);
        }

        public static AtomicInt64 operator -(AtomicInt64 left, Int32 right)
        {
            var rightOprand = 0 - right;
            Interlocked.Add(ref left.value, rightOprand);
            return left;
        }
    }
}
