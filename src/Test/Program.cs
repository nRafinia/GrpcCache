using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using nCache.Client;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            var bDay = new DateTime(1984, 8, 28);
            var age = DateTime.Now.Year - bDay.Year;
            if (bDay > DateTime.Now.AddYears(-age))
                age--;

            var person = new Person()
            {
                Firstname = "Naser",
                Surname = "Rafinia",
                Age = age,
                Test = new P2()
                {
                    Name = "Test"
                }
            };

            var cache = CacheMemory.GetInstance("http://localhost:37532/");
            cache.AddOrUpdate("TestModel", person);

            Person p = null;
            for (var i = 0; i < 10; i++)
            {
                p = cache.Get<Person>("TestModel");
            }

            Console.Write($"Age is equal={person.Age == p?.Age}");
            Console.ReadKey();
        }

    }

    //[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Person
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        public P2 Test { get; set; }
    }

    public class P2
    {
        public string Name { get; set; }
    }
}
