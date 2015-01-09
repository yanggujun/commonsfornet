
using System.Collections;
using System.Collections.Generic;
namespace Commons.Collections.Map
{
	internal class MapEnumerator : IDictionaryEnumerator
	{
		private IDictionary map;
		private IDictionaryEnumerator iter;
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
