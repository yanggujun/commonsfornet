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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections
{
    /// <summary>
    /// The stub interface of map.
    /// </summary>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// TODO: is it necessary to extend serializable?
    [CLSCompliant(true)]
    public interface ITreeMap<TKey, TValue> : IMap<TKey, TValue>
    {
        /// <summary>
        /// Puts the keys of the map to a tree set.
        /// </summary>
        ITreeSet<TKey> KeySet { get; }

        /// <summary>
        /// Return a key value pair whose key is the maximum in the map.
        /// </summary>
        KeyValuePair<TKey, TValue> Max { get; }

        /// <summary>
        /// Return a key value pair whose key is the minimum in the map.
        /// </summary>
        KeyValuePair<TKey, TValue> Min { get; }

        /// <summary>
        /// Removes maximum item in the map.
        /// </summary>
        void RemoveMax();

        /// <summary>
        /// Removes minimum item in the map.
        /// </summary>
        void RemoveMin();
    }
}
