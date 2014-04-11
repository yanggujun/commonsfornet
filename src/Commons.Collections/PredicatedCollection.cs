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

namespace Commons.Collections
{
    /// <summary>
    /// Decorates another collection to validate the conditions to match a specfied predicate.
    /// </summary>
    /// <remarks>
    /// When an object added to the collection cannot pass the validation, the collection throws 
    /// an exception.
    /// For example, the PredicatedCollection can be used to validate the input value of string is not empty:
    /// <code>
    /// var c = new PredicatedCollection<string>(new List<string>, item => !string.IsNullOrEmpty(item));
    /// </code>
    /// </remarks>
    /// <typeparam name="TItem">The type of the collection items.</typeparam>
    // TODO: covariance of TItem?
    [CLSCompliant(true)]
    public class PredicatedCollection<TItem> : AbstractCollectionDecorator<TItem>
    {
        private readonly Func<TItem, bool> predicate;

        public PredicatedCollection(Func<TItem, bool> predicate): this(new List<TItem>(), predicate)
        {

        }

        public PredicatedCollection(ICollection<TItem> collection, Func<TItem, bool> predicate) : base(collection)
        {
            this.predicate = predicate;
        }

        public override void Add(TItem item)
        {
            Validate(item);
            base.Add(item);
        }

        public void AddAll(ICollection<TItem> collection)
        {
            foreach (var item in collection)
            {
                Validate(item);
                Decorated.Add(item);
            }
        }

        protected virtual void Validate(TItem item)
        {
            if (!predicate(item))
            {
                throw new ArgumentException("Cannot validate input object.");
            }
        }
    }
}
