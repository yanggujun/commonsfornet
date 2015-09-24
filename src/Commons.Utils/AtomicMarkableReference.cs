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

namespace Commons.Utils
{
    /// <summary>
    /// Access an reference atomically with the reference marked by a bit.
    /// </summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    [CLSCompliant(true)]
    public class AtomicMarkableReference<T> where T : class
    {
        private AtomicReference<Tuple<T, bool>> value;

        public AtomicMarkableReference(T reference)
            : this(reference, false)
        {
        }

        public AtomicMarkableReference(T reference, bool initialMark)
        {
            value = AtomicReference<Tuple<T, bool>>.From(new Tuple<T, bool>(reference, initialMark));
        }

        public T Value
        {
            get
            {
                return value.Value.Item1;
            }
        }

        public bool IsMarked
        {
            get
            {
                return value.Value.Item2;
            }
        }
    }
}
