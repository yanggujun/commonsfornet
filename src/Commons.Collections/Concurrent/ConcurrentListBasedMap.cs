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
	[CLSCompliant(true)]
	public class ConcurrentListBasedMap<K, V> : IEnumerable<KeyValuePair<K, V>>, IEnumerable
	{
		private Entry head;
		private Entry tail;
		private IComparer<K> comparer;

		public ConcurrentListBasedMap() : this(Comparer<K>.Default)
		{
		}

		public ConcurrentListBasedMap(IComparer<K> comparer)
		{
			this.comparer = comparer;
			head = new Entry();
			tail = new Entry();
			head.Next = tail;
		}

		public bool TryRemove(K key)
		{
			while (true)
			{
				Entry pred, curr;
				Find(key, out pred, out curr);

				pred.Lock();
				try
				{
					curr.Lock();
					try
					{
						if (Validate(pred, curr))
						{
							if (comparer.Compare(curr.Key, key) != 0)
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
			while (true)
			{
				Entry pred, curr;
				Find(key, out pred, out curr);

				pred.Lock();
				try
				{
					curr.Lock();
					try
					{
						if (Validate(pred, curr))
						{
							if (comparer.Compare(curr.Key, key) == 0)
							{
								return false;
							}
							else
							{
								var entry = new Entry {Key = key, Value = value, Marked = false, Next = curr};
								pred.Next = entry;
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

		public bool ContainsKey(K key)
		{
			Entry pred, curr;
			Find(key, out pred, out curr);
			return comparer.Compare(curr.Key, key) == 0 && !curr.Marked;
		}

		public V this[K key]
		{
			get
			{
				Entry pred, curr;
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
				while (true)
				{
					Entry pred, curr;
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

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		private void Find(K key, out Entry pred, out Entry curr)
		{
			pred = head;
			curr = head.Next;
			while (!ReferenceEquals(curr, tail) && comparer.Compare(curr.Key, key) < 0)
			{
				pred = curr;
				curr = curr.Next;
			}
		}

		private bool Validate(Entry pred, Entry curr)
		{
			return !pred.Marked && !curr.Marked && pred.Next == curr;
		}

		private class Entry
		{
			private const int SpinCount = 10;
			private const int YieldCount = 5;
			private const int SpinCycles = 20;
			private AtomicBool locked = AtomicBool.From(false);

			public K Key { get; set; }

			public V Value { get; set; }

			public Entry Next { get; set; }

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
