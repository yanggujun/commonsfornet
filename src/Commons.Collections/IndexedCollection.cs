// Copyright CommonsForNET. Author: Gujun Yang. email: gujun.yang@gmail.com
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections
{
	/// <summary>
	/// The IndexedCollection provides a map like view on a collection. Other than specifying the key and value explicitly,
    /// the indexed collection generates the key from the value added to itself. How the key is generated is defined by 
    /// the client with a delegate.
    /// The inside map is a (key)-(collection of the value) pair.
    /// When unique index is false, items which generate the same key are added to a collection in the IMultiMap dictionary. 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TItem"></typeparam>
    public class IndexedCollection<TKey, TItem> : AbstractCollectionDecorator<TItem>
    {
        private readonly IMultiMap<TKey, TItem> map;
        private readonly Func<TItem, TKey> keyGenerator;

		public IndexedCollection(ICollection<TItem> collection, IMultiMap<TKey, TItem> map, Func<TItem, TKey> keyGenerator) : base(collection)
        {
            this.map = map;
            this.keyGenerator = keyGenerator;
        }

    }
}
