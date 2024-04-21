using System;
using System.Text.Json.Serialization;

namespace TowardAgarioStepTwo
{
    [JsonDerivedType(typeof(Person), "Person")]
    [JsonDerivedType(typeof(Student), "Student")]
    public class Person
    {
        public static int NextID = 1;
        public int ID { get; private set; }
        public float GPA { get; private set; }
        public string Name { get; private set; }

        public Person(string name, float gpa)
        {
            ID = NextID++;
            Name = name;
            GPA = gpa;
        }

        public Person()
        {
            ID = 1;
            GPA = 4;
            Name = "Jim";
        }

        public Person(string name)
        {
            Name = name;
        }
    }
}

