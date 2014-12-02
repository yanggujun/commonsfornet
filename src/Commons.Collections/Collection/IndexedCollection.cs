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
using System.Collections.Generic;

using Commons.Collections.Map;
using Commons.Utils;

namespace Commons.Collections.Collection
{
    /// <summary>
    /// The IndexedCollection provides a map like view on a collection. Other than specifying the key and value explicitly,
    /// the indexed collection generates the key from the value added to itself. How the key is generated is defined by 
    /// the client with a delegate.
    /// The inside map is a (key)-(collection of the value) pair.
    /// When unique index is false, items which generate the same key are added to a collection in the IMultiMap dictionary. 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [CLSCompliant(true)]
    public class IndexedCollection<K, V> : AbstractCollectionDecorator<V>
    {
        private readonly IMultiMap<K, V> map;
        private readonly Transformer<V, K> transform;

        public IndexedCollection(ICollection<V> valueCollection, Transformer<V, K> transform) : base(valueCollection)
        {
            this.transform = transform;
        }

        public IndexedCollection(ICollection<V> valueCollection, IMultiMap<K, V> map, Transformer<V, K> transform)
            : base(valueCollection)
        {
            this.transform = transform;
            this.map = map;
        }

    }
}
