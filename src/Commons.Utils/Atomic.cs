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

using System.Threading;
namespace Commons.Utils
{
    /// <summary>
    /// The struct provides the atomic operations for a reference type <typeparamref name="T"/>.
    /// .NET framework has the x86 and x64 CAS implementation on windows. 
    /// While on Mono, the implementation is different. The class is to encapsulate the CAS operation 
    /// on different CPU architectures and different OS.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Atomic<T> where T : class
    {
        private T reference;

        public static Atomic<T> From(T reference)
        {
            return new Atomic<T>(reference);
        }

        private Atomic(T reference)
        {
            this.reference = reference;
        }

        public bool CompareExchange(T newValue)
        {
            var old = Interlocked.CompareExchange(ref reference, newValue, reference);
            return !ReferenceEquals(old, reference);
        }

        public bool Exchange(T newValue)
        {
            var old = Interlocked.Exchange(ref reference, newValue);
            return !ReferenceEquals(old, reference);
        }

        public T Value { get { return reference; } }
    }
}
