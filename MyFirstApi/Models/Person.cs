using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.Models
{
    public class Person
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Range(0,120)]
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}
