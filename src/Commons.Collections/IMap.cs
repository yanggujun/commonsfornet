using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections
{
    [CLSCompliant(true)]
    public interface IMap<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>,
                                          IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>,
                                          IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
    }
}
