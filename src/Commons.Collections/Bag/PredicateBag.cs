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

namespace Commons.Collections.Bag
{
    [CLSCompliant(true)]
    public class PredicateBag<T> : AbstractBagDecorator<T>
    {
        private readonly Predicate<T> predicate;

        public PredicateBag(IBag<T> bag, Predicate<T> predicate)
            : base(bag)
        {
            this.predicate = predicate;
        }

        public override void Add(T item, int copies = 1)
        {
            Validate(item);
            base.Add(item, copies);
        }

        public override bool Remove(T item, int copies = 1)
        {
            Validate(item);
            return base.Remove(item, copies);
        }

        public override bool RemoveAll(ICollection<T> collection)
        {
            foreach (var item in collection)
            {
                Validate(item);
            }
            return base.RemoveAll(collection);
        }

        public override bool RetainAll(ICollection<T> collection)
        {
            foreach (var item in collection)
            {
                Validate(item);
            }
            return base.RetainAll(collection);
        }

        protected virtual void Validate(T item)
        {
            if (predicate(item))
            {
                throw new ArgumentException("The input item cannot be validated");
            }
        }
    }
}
