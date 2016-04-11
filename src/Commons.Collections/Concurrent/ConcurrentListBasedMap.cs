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
using System.Threading;
using Commons.Collections.Collection;
using Commons.Utils;

namespace Commons.Collections.Concurrent
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    //[CLSCompliant(true)]
    internal class ConcurrentListBasedMap<K, V> : IEnumerable<KeyValuePair<K, V>>, IEnumerable
    {
        private LockableNode head;
        private LockableNode tail;
        private IComparer<K> comparer;

        public ConcurrentListBasedMap() : this(Comparer<K>.Default)
        {
        }

        public ConcurrentListBasedMap(IComparer<K> comparer)
        {
            Guarder.CheckNull(comparer, "comparer");
            this.comparer = comparer;
            head = new LockableNode();
            tail = new LockableNode();
            head.Next = tail;
        }

        public bool TryRemove(K key)
        {
            Guarder.CheckNull(comparer, "key");
            while (true)
            {
                LockableNode pred, curr;
                Find(key, out pred, out curr);

                pred.Lock();
                try
                {
                    curr.Lock();
                    try
                    {
                        if (Validate(pred, curr))
                        {
                            if ((ReferenceEquals(pred.Next, tail) && ReferenceEquals(curr, tail)) || comparer.Compare(curr.Key, key) != 0)
                            {
                                return false;
                            }
                            else
                            {
                                curr.Marked = true;
                                pred.Next = curr.Next;
                                return true;
                            }
                        }
                    }
                    finally
                    {
                        curr.Unlock();
                    }
                }
                finally
                {
                    pred.Unlock();
                }
            }
        }

        public bool TryAdd(K key, V value)
        {
            Guarder.CheckNull(key, "key");
            Guarder.CheckNull(value, "value");
            while (true)
            {
                LockableNode pred, curr;
                Find(key, out pred, out curr);

                pred.Lock();
                try
                {
                    curr.Lock();
                    try
                    {
                        if (Validate(pred, curr))
                        {
                            if ((ReferenceEquals(pred.Next, tail) && ReferenceEquals(curr, tail)) || (comparer.Compare(curr.Key, key) != 0))
                            {
                                var entry = new LockableNode {Key = key, Value = value, Marked = false, Next = curr};
                                pred.Next = entry;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    finally
                    {
                        curr.Unlock();
                    }
                }
                finally
                {
                    pred.Unlock();
                }
            }
        }

        public bool ContainsKey(K key)
        {
            Guarder.CheckNull(key, "key");
            LockableNode pred, curr;
            Find(key, out pred, out curr);
            return !IsNotFound(pred, curr) && (comparer.Compare(curr.Key, key) == 0 && !curr.Marked);
        }

        public bool TryGetValue(K key, out V value)
        {
            Guarder.CheckNull(key, "key");
            LockableNode pred, curr;
            Find(key, out pred, out curr);
            value = default(V);
            var found = false;
            if (!IsNotFound(pred, curr) && (comparer.Compare(curr.Key, key) == 0 && !curr.Marked))
            {
                value = curr.Value;
                found = true;
            }

            return found;
        }

        public void Clear()
        {
            head.Lock();
            try
            {
                head.Next = tail;
            }
            finally
            {
                head.Unlock();
            }
        }

        public V this[K key]
        {
            get
            {
                Guarder.CheckNull(key, "key");
                LockableNode pred, curr;
                Find(key, out pred, out curr);
                if (comparer.Compare(curr.Key, key) == 0 && !curr.Marked)
                {
                    return curr.Value;
                }
                else
                {
                    throw new KeyNotFoundException(string.Format(Messages.KeyDoesNotExistInMap, key));
                }
            }
            set
            {
                Guarder.CheckNull(key, "key");
                while (true)
                {
                    LockableNode pred, curr;
                    Find(key, out pred, out curr);
                    curr.Lock();
                    try
                    {
                        if (comparer.Compare(curr.Key, key) == 0 && !curr.Marked)
                        {
                            curr.Value = value;
                            break;
                        }
                    }
                    finally
                    {
                        curr.Unlock();
                    }
                    if (TryAdd(key, value))
                    {
                        break;
                    }
                }
            }
        }

        public KeyValuePair<K, V> Min
        {
            get
            {
                while (true)
                {
                    var min = head.Next;
                    if (ReferenceEquals(min, tail))
                    {
                        throw new InvalidOperationException(Messages.CollectionEmpty);
                    }
                    if (!min.Marked)
                    {
                        return new KeyValuePair<K, V>(min.Key, min.Value);
                    }
                }
            }
        }

        public KeyValuePair<K, V> Max
        {
            get
            {
                while (true)
                {
                    var max = head;
                    while (!ReferenceEquals(max.Next, tail))
                    {
                        max = max.Next;
                    }
                    if (ReferenceEquals(max, head))
                    {
                        throw new InvalidOperationException(Messages.CollectionEmpty);
                    }
                    if (!max.Marked)
                    {
                        return new KeyValuePair<K, V>(max.Key, max.Value);
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                var count = 0;
                var curr = head.Next;
                while (!ReferenceEquals(curr, tail))
                {
                    if (!curr.Marked)
                    {
                        count++;
                    }
                    curr = curr.Next;
                }

                return count;
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            var curr = head.Next;
            while (!ReferenceEquals(curr, tail))
            {
                if (!curr.Marked)
                {
                    yield return new KeyValuePair<K, V>(curr.Key, curr.Value);
                    curr = curr.Next;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Find(K key, out LockableNode pred, out LockableNode curr)
        {
            pred = head;
            curr = head.Next;
            while (!ReferenceEquals(curr, tail) && comparer.Compare(curr.Key, key) < 0)
            {
                pred = curr;
                curr = curr.Next;
            }
        }

        private bool IsNotFound(LockableNode pred, LockableNode curr)
        {
            return ReferenceEquals(pred.Next, tail) && ReferenceEquals(curr, tail);
        }

        private bool Validate(LockableNode pred, LockableNode curr)
        {
            return !pred.Marked && !curr.Marked && pred.Next == curr;
        }

        private class LockableNode
        {
            private const int SpinCount = 10;
            private const int YieldCount = 5;
            private const int SpinCycles = 20;
            private AtomicBool locked = AtomicBool.From(false);

            public K Key { get; set; }

            public V Value { get; set; }

            public LockableNode Next { get; set; }

            public bool Marked { get; set; }

            public void Lock()
            {
                if (!locked.CompareExchange(true, false))
                {
                    SpinLock();
                }
            }

            public void Unlock()
            {
                locked.Value = false;
            }

            private void SpinLock()
            {
                var pc = Environment.ProcessorCount;
                for (var i = 0;; i++)
                {
                    if (pc > 1 && i < SpinCount)
                    {
                        Thread.SpinWait(SpinCycles * ( i + 1));
                    }
                    else if (i < (SpinCount + YieldCount))
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                    if (!locked.Value && locked.CompareExchange(true, false))
                    {
                        break;
                    }
                }
            }
        }
    }
}
