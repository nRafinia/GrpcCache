using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GrpcCache.Client;

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

            var test = new P2()
            {
                Name = "Test"
            };
            var person = new Person()
            {
                Firstname = "Naser",
                Surname = "Rafinia",
                Age = age,
                Test = test
            };

            var cache = CacheMemory.GetInstance("http://localhost:37532/");
            cache.AddOrUpdate("TestModel", person);
            cache.AddOrUpdate("TestModel1", person);
            cache.AddOrUpdate("Test", test);

            var status = cache.GetStatus();

            var p = cache.Get<Person>("TestModel");
            var items = cache.GetAll<Person>("");
            var exists = cache.Exists<Person>("TestModel");
            
            cache.Remove<Person>("TestModel");
            items = cache.GetAll<Person>("");
            exists = cache.Exists<Person>("TestModel");
            
            cache.RemoveAll<Person>();
            items = cache.GetAll<Person>("");

            status = cache.GetStatus();

            var t = cache.Get<P2>("Test");
            cache.RemoveAll();
            exists = cache.Exists<P2>("Test");

            status = cache.GetStatus();

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
