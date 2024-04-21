using System.Text.Json;
using TowardAgarioStepTwo;

var person = new Person();
var message = JsonSerializer.Serialize(person);
Console.WriteLine("Serialized person: " + message);
var temp = JsonSerializer.Deserialize<Person>(message) ?? new Person();

var people = new List<Person>
{
    new Person("Jim", 3.0f),
    new Person("Dav", 3.3f),
     new Person("JAav", 3.5f),
      new Person("PDav", 3.6f),
       new Person("EADav", 3.9f),
};

var options = new JsonSerializerOptions { WriteIndented = true };
var peopleJson = JsonSerializer.Serialize(people, options);
Console.WriteLine("Serialized people: " + peopleJson);

var student = new Student("Jim", 3.8f);

var options1 = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
string jsonString = JsonSerializer.Serialize<Person>(student, options);

Console.WriteLine(jsonString);

Person deserializedPerson = JsonSerializer.Deserialize<Person>(jsonString, options1);

// Inspect the deserialized object
Console.WriteLine($"Deserialized Name: {deserializedPerson.Name}");