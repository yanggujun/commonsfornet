###Commons.Json

A lightweight library for JSON manipulation.

  *Map the class to a JSON string

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
  
  *Map a JSON string to a .NET object

  ```csharp
    // json = {"Name":"Joe","Age":25, "Nationality":"US","Gender":"Male"}
    var person = JsonMapper.To<Person>(json);
  ```

  *The mapper ignores the case
  ```csharp
    // json = {"name":"Joe","AGE":25, "nationality":"US","gender":"Male"}
    var person = JsonMapper.To<Person>(json); // still get the same object with the person object defined priviously
  ```

  *Map a JSON array to list. Note: only List<T> and T[] are supported for now.

  ```csharp
    // json = [{"Name":"Joe","Age":25, "Nationality":"US","Gender":"Male"}, {"Name":"John", "Age":27, "Nationality":"FR", "Gender":"Male"}, {"Name":"Jane","Age":23, "Nationality":"EN","Gender":"Female"}]
    var list = JsonMapper.To<List<Person>>(json);
  ```

  *Map a collection to JSON array. You can map most of collection into a JSON array or a JSON object (for dictionary<string, string>)

  ```csharp
    var jane = new Person{...};
    var john = new Person{...};
    var joe = new person{...};
    var list = new List<Person>{joe, john, jane};
    var json = JsonMapper.ToJson(list);
  ```

  *Map JSON primitives to .NET primtives and string

  ```csharp
    // json = 5
    var number = JsonMapper.To<int>(json); // number = 5

    var anotherNumber = JsonMapper.To<long>(json); // anotherNumber = 5

    // json = 5.6
    var floatingNumber = JsonMapper.To<double>(json); // floatingNumber = 5.6

    // note: this will fail when json = 5.6
    var wrongNumber = JsonMapper.To<int>(json);

    // json = "a plain string"
    var str = JsonMapper.To<string>(json); // str = "a plain string"

    // json = true
    var b = JsonMapper.To<bool>(json); // b = true
  ```

  *Map .NET primitives and string to JSON

  *Map an enum

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

    // json = "Art"
    var major = JsonMapper.To<Major>(json); // major = Major.Art

    // Map the enum to JSON
    var json = JsonMapper.ToJson(Major.Physics); // json = "Physics"
  ```

  *Combine those things together...

  *Only property with public getters are mapped from object to JSON

  *Only property with public setters are mapped from JSON to object

  *When no corresponding property is found on the object, it is ignored.

  *You can tell the mapper to map an object property into a JSON key

  *And not to map the property

  *Use a specific date time format

  *And use the default date time format

  *Write your own JSON-Object converter
