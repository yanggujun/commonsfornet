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

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public abstract class AbstractBagDecorator<T> : AbstractCollectionDecorator<T>, IBag<T>
    {
        protected AbstractBagDecorator(IBag<T> bag) : base(bag)
        {
        }

        protected new IBag<T> Decorated { get { return (IBag<T>)base.Decorated; } }

        public int GetCount(T item)
        {
            return Decorated.GetCount(item);
        }

        public void Add(T item, int copies = 1)
        {
            Decorated.Add(item, copies);
        }

        public bool Remove(T item, int copies = 1)
        {
            return Decorated.Remove(item, copies);
        }

        public ITreeSet<T> UniqueSet()
        {
            return Decorated.UniqueSet();
        }

        public bool ContainsAll(ICollection<T> collection)
        {
            return Decorated.ContainsAll(collection);
        }

        public bool RemoveAll(ICollection<T> collection)
        {
            return Decorated.RemoveAll(collection);
        }

        public bool RetainAll(ICollection<T> collection)
        {
            return Decorated.RetainAll(collection);
        }
    }
}
