using System;
namespace TowardAgarioStepTwo
{
    public class Student : Person
    {
        public float GPA { get; set; } // GPA is now part of Student

       
        public Student(string name, float gpa) : base(name)
        {
            GPA = gpa;
        }
    }
}

