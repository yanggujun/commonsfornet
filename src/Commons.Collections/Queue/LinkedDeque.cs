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

using Commons.Utils;
using Commons.Collections.Collection;

namespace Commons.Collections.Queue
{
	/// <summary>
	/// A LinkedDeque is a deque with its entries doubly linked with their next and previous siblings.
	/// </summary>
	/// <typeparam name="T">The type of the item in the deque.</typeparam>
    [CLSCompliant(true)]
	public class LinkedDeque<T> : IDeque<T>, IEnumerable<T>, ICollection, IEnumerable
	{
        private DoubleLinkedEntry<T> Header { get; set; }

		public void Append(T item)
		{
            if (Header == null)
            {
                MakeHeader(item);
            }
            else
            {
                var newEntry = new DoubleLinkedEntry<T>();
                newEntry.Entry = item;
                newEntry.Previous = Header.Previous;
                Header.Previous.Next = newEntry;
                Header.Previous = newEntry;
                newEntry.Next = Header;
            }
		}

		public void Prepend(T item)
		{
            Append(item);
            Header = Header.Previous;
		}

		public T Pop()
		{
            Header.Validate(x => x != null, new InvalidOperationException(Messages.DequeEmptyError));
            var item = default(T);
            if (Header == Header.Previous && Header == Header.Next)
            {
                item = Header.Entry;
                Header = null;
            }
            else
            {
                var last = Header.Previous;
                item = last.Entry;
                Header.Previous = last.Previous;
                last.Previous.Next = Header;
                last.Previous = null;
                last.Next = null;
            }

            return item;
		}

		public T Shift()
		{
            Header.Validate(x => x != null, new InvalidOperationException(Messages.DequeEmptyError));
            var item = default(T);

            if (Header == Header.Previous && Header == Header.Next)
            {
                item = Header.Entry;
                Header = null;
            }
            else
            {
                var second = Header.Next;
                var last = Header.Previous;
                var first = Header;
                item = Header.Entry;
                last.Next = second;
                second.Previous = last;
                Header = second;
                first.Previous = null;
                first.Next = null;
            }

            return item;
		}

		public T First
		{
            get
            {
                Header.Validate(x => x != null, new InvalidOperationException(Messages.DequeEmptyError));
                return Header.Entry;
            }
		}

		public T Last
		{
            get
            {
                Header.Validate(x => x != null, new InvalidOperationException(Messages.DequeEmptyError));
                return Header.Previous.Entry;
            }
		}

        public bool IsEmpty { get { return Header == null; } }

		public IEnumerator<T> GetEnumerator()
		{
            return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
            return GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
            array.ValidateNotNull(Messages.ArgumentArray);
            var items = (T[])array;
            var cursor = 0;
            foreach (var item in Items)
            {
                items[index + (cursor++)] = item;
            }
		}

		public int Count
		{
            get
            {
                var count = 0;
                foreach(var item in Items)
                {
                    count++;
                }

                return count;
            }
		}

		bool ICollection.IsSynchronized
		{
            get { return false; }
		}

		object ICollection.SyncRoot
		{
            get { throw new NotSupportedException(Messages.SyncRootNotSupportError); }
		}

        private void MakeHeader(T item)
        {
            Header = new DoubleLinkedEntry<T>();
            Header.Entry = item;
            Header.Next = Header;
            Header.Previous = Header;
        }

        private IEnumerable<T> Items
        {
            get
            {
                if (Header != null)
                {
                    var cursor = Header;
                    do
                    {
                        yield return cursor.Entry;
                        cursor = cursor.Next;
                    } while (cursor != Header);
                }
            }
        }
	}
}
