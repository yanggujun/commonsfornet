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
    public struct AtomicBool
    {
        private volatile int value;

        private AtomicBool(bool initialValue)
        {
            value = initialValue ? 1 : 0;
        }

        public static AtomicBool From(bool initialValue)
        {
            return new AtomicBool(initialValue);
        }

        public bool Value
        {
            get
            {
                return value == 1;
            }
        }

#pragma warning disable 420
        public bool CompareExchange(bool newValue, bool oldValue)
        {
            var o = oldValue ? 1 : 0;
            var n = newValue ? 1 : 0;
            return Interlocked.CompareExchange(ref value, n, o) == o;
        }

        public bool Exchange(bool newValue)
        {
            var n = newValue ? 1 : 0;
            var o = value;
            var r = Interlocked.Exchange(ref value, n);
            return r == o;
        }
#pragma warning restore 420

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator bool(AtomicBool atomic)
        {
            return atomic.Value;
        }
    }
}
