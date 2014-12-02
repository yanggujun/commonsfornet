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
using System.Collections.Generic;

namespace Commons.Collections.Collection
{
    /// <summary>
    /// The collection decorator for the concrete collections.
    /// The abstract class just calls the decorator for every operation.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    [CLSCompliant(true)]
    public class AbstractCollectionDecorator<T> : ICollection<T>
    {
        /// <summary>
        /// The decorated collection.
        /// </summary>
        protected ICollection<T> Decorated { get; private set; }
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="collection">The collection to decorate.</param>
        protected AbstractCollectionDecorator(ICollection<T> collection)
        {
            if (null == collection)
            {
                throw new ArgumentException("Collection cannot be null");
            }
            Decorated = collection;
        }

        public virtual void Add(T item)
        {
            Decorated.Add(item);
        }

        public virtual void Clear()
        {
            Decorated.Clear();
        }

        public virtual bool Contains(T item)
        {
            return Decorated.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Decorated.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return Decorated.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return Decorated.IsReadOnly; }
        }

        public virtual bool Remove(T item)
        {
            return Decorated.Remove(item);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return Decorated.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Decorated.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return Decorated.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Decorated.GetHashCode();
        }

        public override string ToString()
        {
            return Decorated.ToString();
        }
    }
}
