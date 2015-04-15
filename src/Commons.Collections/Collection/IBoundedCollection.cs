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
using System.Collections;

namespace Commons.Collections.Collection
{
    /// <summary>
    /// Defines a collection that is size limited. The size of the collection can vary, but it 
    /// cannot exceed the mas size of the collection.
    /// The max size of the collection is pre-defined. It cannot be changed in the afterwards.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(true)]
    public interface IBoundedCollection : ICollection, IEnumerable
    {
        /// <summary>
        /// Indicates whether the collection is full. If the collection is full, 
        /// adding operation throws OperationNotSupportedException.
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// Returns the max size of the collection.
        /// </summary>
        int MaxSize { get; }
    }
}
