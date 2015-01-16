// Copyright CommonsForNET 2014.
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

using System.Collections;
using System.Collections.Generic;

namespace Commons.Collections.Map
{
	internal class MapEnumerator : IDictionaryEnumerator
	{
		private readonly IDictionary map;
		private readonly IDictionaryEnumerator iter;
		public MapEnumerator(IDictionary map)
		{
			this.map = map;
			iter = this.map.GetEnumerator();
		}
		public DictionaryEntry Entry
		{
			get
			{
				var entry = new DictionaryEntry(iter.Key, iter.Value);
				return entry;
			}
		}

		public object Key
		{
			get
			{
				return Entry.Key;
			}
		}

		public object Value
		{
			get
			{
				return Entry.Value;
			}
		}

		public object Current
		{
			get
			{
				return Entry;
			}
		}

		public bool MoveNext()
		{
			return iter.MoveNext();
		}

		public void Reset()
		{
			iter.Reset();
		}
	}
}
