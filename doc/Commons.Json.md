###Commons.Json

A lightweight library to map between .NET objects and JSON fluently and dynamically.

  __Map the object to a JSON string__

  ```csharp
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
    }
    
    var person = new Person
    {
        Name = "Joe",
        Age = 25,
        Nationality = "US",
        Gender = "Male"
    };
    var json = JsonMapper.ToJson(person); // {"Name":"Joe","Age":25,"Nationality":"US","Gender":"Male"}
  ```
  
  __Map a JSON string to a .NET object__

  ```csharp
    // json string: {"Name":"Joe","Age":25, "Nationality":"US","Gender":"Male"}
    var person = JsonMapper.To<Person>(json);
  ```

  __The mapper ignores the case__
  ```csharp
    // json string: {"name":"Joe","AGE":25, "nationality":"US","gender":"Male"}
    var person = JsonMapper.To<Person>(json); // still get the same object with the person object defined priviously
  ```

  __Map a JSON array to list__

  ```csharp
    // json string: [{"Name":"Joe","Age":25, "Nationality":"US","Gender":"Male"}, {"Name":"John", "Age":27, "Nationality":"FR", "Gender":"Male"}, {"Name":"Jane","Age":23, "Nationality":"EN","Gender":"Female"}]
    var list = JsonMapper.To<List<Person>>(json);
  ```

  Note: only List<T> and T[] are supported for now.

  __Map a JSON array to .NET array__

  ```csharp
    // json string : [0, 1, 3, 4, 5, 6]
    var array = JsonMapper.To<int[]>(json); // get an int array with value [0, 1, 2, 3, 4, 5, 6]

    // json string :[[0, 1, 2], [3, 4, 5], [6, 7, 8]]
    var matrix = JsonMapper.To<int[][]>(json); // get an int matrix object.
  ```

  __Map a collection to JSON array. You can map most of collection into a JSON array or a JSON object (for dictionary<string, T>)__

  ```csharp
    var jane = new Person{...};
    var john = new Person{...};
    var joe = new person{...};
    var list = new List<Person>{joe, john, jane};
    var json = JsonMapper.ToJson(list);
  ```

  __Map JSON primitives to .NET primtives and string__

  ```csharp
    // json string: 5
    var number = JsonMapper.To<int>(json); // number = 5

    var anotherNumber = JsonMapper.To<long>(json); // anotherNumber = 5

    // json string: 5.6
    var floatingNumber = JsonMapper.To<double>(json); // floatingNumber = 5.6

    // note: this will fail when json string is 5.6
    var wrongNumber = JsonMapper.To<int>(json);

    // json string: "a plain string"
    var str = JsonMapper.To<string>(json); // str : "a plain string"

    // json = true
    var b = JsonMapper.To<bool>(json); // b : true
  ```

  __Map .NET primitives and string to JSON__

  ```csharp
    var number = 100;
    var json = JsonMapper.ToJson(number); // json string: 100

    var floatingNumbe = 5.1274;
    var json = JsonMapper.ToJson(floatingNumber); // json string: 5.1274

    var str = "the demo string";
    var json = JsonMapper.ToJson(str); // json string: "the demo string"
  ```

  __Map an enum__

  ```csharp
	public enum Major
	{
		CS,
		Art,
		Politics,
		Economics,
		Physics,
		Chemistry
	}

    //...

    // json string: "Art"
    var major = JsonMapper.To<Major>(json); // major = Major.Art

    // Map the enum to JSON
    var json = JsonMapper.ToJson(Major.Physics); // json = "Physics"
  ```

  __Combine those things together...__

  __Only property with public getters are mapped from object to JSON__

  __Only property with public setters are mapped from JSON to object__

  __When no corresponding property is found on the object, it is ignored.__

  __You can tell the mapper to map an object property into a different name__ 

  __And not to map the property__

  __Use a specific date time format__

  __And use the default date time format__

  Now the date time format is univernal. In case you need to switch the date time format for some objects.

  __Write your own JSON-Object converter__

  __Work with dynamic JSON object, array and primitives__
