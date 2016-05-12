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

        public Int64 Increment()
        {
            return Interlocked.Increment(ref value);
        }

        public Int64 Decrement()
        {
            return Interlocked.Decrement(ref value);
        }

        public Int64 Add(Int64 right)
        {
            return Interlocked.Add(ref value, right);
        }

        public Int64 Minus(Int64 right)
        {
            var oprand = -right;
            return Interlocked.Add(ref value, oprand);
        }
    }
}
