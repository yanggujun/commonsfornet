##.NET Commons Library

####About

.NET Commons Library is intented to build common components for .NET applications. It is originally inspired by the apache commons implementation for java applications. The .NET Commons Library will be implemented according to the components in apache commons but will not be limited to it.

####Release Notes V0.1.0

Initial release for .NET Commons Library.

#####Commons.Utils

  -Supported hash functions: 

    Murmur hash 32bit
    
    Fnv hash
    
  -Functors
  
    --Equator
    
    --Closure
    
    --Transformer
    
    --Factory
    
  -Guarder, A common utility to check parameters
  
#####Commons.Collections

  -HashedMap, a hash map (dictionary)
  
  -Customized32HashedMap, a hash map (dictionary) whose hash function can be defined other than using Object.GetHashCode()
  
  -TreeMap, a sorted map (dictionary). The map is navigable.
  
  -SkipListMap, a sorted map (dictionary). The map is navigable.
  
  -LruMap, a hash map (dictionary) implemented with least recently used algorithm. The map size is bounded and the oldest item is removed when the map size is full.
  
  -ReferenceMap, a map (dictionary) only takes the object reference value for hash.
  
  -LinkedHashedMap, an ordered map (dictionary) which remembers the adding sequence of the items.
  
  -HashedBimap, an unsorted bidirectional map. A bidirectional map is a map which can be used to find a value from a key and a key from a value.
  
  -TreeBimap, a sorted bidirectional map. 
  
  -MultiValueHashedMap, a unsorted map which multiple values are available for a given key.
  
  -MultiValueTreeMap, a sorted multi value map.
  
  -HashedBag, a bag containing the unsorted items. A bag is a collection that allows duplicate values.
  
  -TreeBag, a bag containing the sorted items.
  
  -Deque, a double ended queue.
  
  -MaxPriorityQueue, a priority queue where the maximum item is at the top.
  
  -MinPriorityQueue, a priority queue where the minimum item is at the top.
  
  -BoundedQueue, a FIFO queue with a maximum size.
  
  -HashedSet, a set with unsorted items. It's a set strictly follows the definition in math.
  
  -TreeSet, a set with sorted items. It's a set strictly follows the definition in math.
  
  -SkipListSet, a set with sorted items. It's a set strictly follows the definition in math.
  
#####Commons.Json

  -A JSON parser and composer with the power of "dynamic" keyword.
  
  -usage:
  
    given the following json text:
    
    var text = @'
    {
    
      "EPL": 
      
        {
        
          "ClubNumber": 20,
          
          "LastChampion": "ManCity"
          
        },
        
      "WorldCup":
      
        {
        
          "Host": "Brazil",
          
          "TeamNumber": 32,
          
          "Champion": "Germany"
          
        }
    }';
    
    dynamic json = JsonObject.Parse(text);
    
    Console.WriteLine(json.EPL.ClubNumber); // output : 20
    
    Console.WriteLine(json.WorldCup.Host); // output: Brazil
    

