// Copyright CommonsForNET 2014.
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

namespace Commons.Collections.Map
{
    [CLSCompliant(true)]
    public class LinkedMap<K, V> : AbstractHashMap<K, V>, IOrderedMap<K, V>
    {
        public LinkedMap()
            : base(0, null, null)
        {
        }

        protected override int CalculateCapacity(int proposedCapacity)
        {
            throw new NotImplementedException();
        }

        protected override long HashIndex(K key)
        {
            throw new NotImplementedException();
        }

        public K FirstKey
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.Generic.KeyValuePair<K, V> First
        {
            get { throw new NotImplementedException(); }
        }

        public K LastKey
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.Generic.KeyValuePair<K, V> Last
        {
            get { throw new NotImplementedException(); }
        }

        public K NextOf(K key)
        {
            throw new NotImplementedException();
        }

        public K PreviousOf(K key)
        {
            throw new NotImplementedException();
        }
    }
}
