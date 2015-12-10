##.NET Commons

###About

.NET Commons is originally inspired by the "apache commons project" for java applications. It is intended to provide the common components and basic application facilities for .NET applications.

###License

Apache License v2.0

###Installation

The lastest release source code can be found on

https://github.com/yanggujun/commonsfornet/releases

__Commons__
    
The package includes Commons.Utils, Commons.Collections and Commons.Json

The nuget package can be retrieved from 

https://www.nuget.org/packages/Commons/

Or on nuget package manager console, type:

      PM> Install-Package Commons

 __Commons.Pool__
    
The nuget package can be retrieved from 

https://www.nuget.org/packages/Commons.Pool/

Or on nuget package manager console, type:

      PM> Install-Package Commons.Pool

###Issues

Please report issues to 

https://github.com/yanggujun/commonsfornet/issues

or send an email to: alanier@sina.cn

###Release Notes V0.2

Initial release for .NET Commons Library. Support .NET framework 4.0 and 4.5.

####Commons.Utils

  * __Atomic Primitives__

    The class libary provides the following atomic primitives for concurrent computing. Those .NET structures enable the compare and swap (CAS) or compare exchange capability.
  
    * AtomicBool

    * AtomicDouble

    * AtomicFloat

    * AtomicInt32

    * AtomicInt64

    * AtomicReference

    * AtomicMarkableReference, a reference type with a flag incidating the state of the reference. A C# counterpart of java.util.concurrent.atomic.AtomicMarkableReference<T>

  * __Supported hash functions__

    Murmur hash 32bit
    
    Fnv hash
    
  * __Functors__
  
    * Equator
    
    * Closure
    
    * Transformer
    
    * Factory
    
  * __Guarder__, A common utility to check parameters
  
####Commons.Collections

  * __HashedMap__, a hash map (dictionary).
  
  * __Customized32HashedMap__, a hash map (dictionary) whose hash function can be defined other than using Object.GetHashCode().
  
  * __TreeMap__, a sorted map (dictionary). The map is navigable.
  
  * __SkipListMap__, a sorted map (dictionary). The map is navigable.
  
  * __LruMap__, a hash map (dictionary) implemented with least recently used algorithm. The map size is bounded and the oldest item is eliminated when the map size is full. 
  
  * __ReferenceMap__, a hash map (dictionary) which calculates the hash value only from its key object reference value.
  
  * __LinkedHashedMap__, an ordered map (dictionary) which remembers the adding sequence of the items.
  
  * __HashedBimap__, an unsorted bidirectional map. A bidirectional map is a map which can be used to find a value from a key and a key from a value.
  
  * __TreeBimap__, a sorted bidirectional map. 
  
  * __MultiValueHashedMap__, an unsorted map where multiple values are allowed for a given key.
  
  * __MultiValueTreeMap__, a sorted version of multi-value map.
  
  * __HashedBag__, a bag containing the unsorted items. A bag is a collection that allows duplicate values.
  
  * __TreeBag__, a bag containing the sorted items.
  
  * __Deque__, a double ended queue.

  * __LinkedDeque__, a double ended queue with linked list as inside data structure.
  
  * __MaxPriorityQueue__, a priority queue where the maximum item is at the top.
  
  * __MinPriorityQueue__, a priority queue where the minimum item is at the top.
  
  * __BoundedQueue__, a FIFO queue with a maximum size.
  
  * __HashedSet__, a set with unsorted items. It's a set strictly follows the definition in maths.
  
  * __TreeSet__, a set with sorted items. It's a set strictly follows the definition in maths.
  
  * __SkipListSet__, a set with sorted items. It's a set strictly follows the definition in maths.

  * __ReferenceSet__, a hash set which calculates the hash value only from its object reference value.
  
  * __LinkedSet__, an ordered set where the elements are sequenced by the adding order.

  * __LruSet__, an ordered set where the elements are sequenced by the least recently used algorithm. The set is bounded and the oldest item is eliminated when the size is full.
  
####Commons.Json

  * A JSON parser and composer with the power of "dynamic" keyword.
  
  * Parse and compose a JSON string with Commons.Json:
  
  ```csharp
      //given the following json text:
      var text = @' { "EPL": { "ClubNumber": 20, "LastChampion": "ManCity" }, "WorldCup": { "Host": "Brazil", "TeamNumber": 32, "Champion": "Germany" } }';
      dynamic json = JsonObject.Parse(text);
      Console.WriteLine(json.EPL.ClubNumber); // output : 20
      Console.WriteLine(json.WorldCup.Host); // output: Brazil

      //Compose a JSON string:
      dynamic jsonObj = new JsonObject();
      jsonObj.EPL.ClubNumber = 20;
      jsonObj.EPL.LastChampion = "ManCity";
      Console.WriteLine(jsonObj) // output : { "EPL": { "ClubNumber": 20, "LastChampion": "ManCity"} } 
  ```

####Commons.Pool

  * A light weight generic object pool.
  
  ```csharp
      var sqlFactory = new DefaultDbConnectionFactory(connectionString);
      var connectionPool = new GenericObjectPool<IDbConnection>(0, 10, sqlFactory);
      //...
      var connection = connectionPool.Acquire();
      // use the connection.
      connectionPool.Return(connection);
      //...
      connectionPool.Dispose();
  ```
