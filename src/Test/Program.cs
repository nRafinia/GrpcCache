using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FASTER.core;
using GrpcCache.Client;
using Utf8Json;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Test2();
            //Test3();
            //return;

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
                Id=100,
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

        private static void Test2()
        {
            // This sample uses class key and value types, which are not blittable (i.e., they
            // require a pointer to heap objects). Such datatypes include variable length types
            // such as strings. You can override the default key equality comparer in two ways;
            // (1) Make Key implement IFasterEqualityComparer<Key> interface
            // (2) Provide IFasterEqualityComparer<Key> instance as param to constructor
            // FASTER stores the actual objects in a separate object log.
            // Note that serializers are required for class types, see below.

            var dir = Path.GetTempPath()+"\\Faster";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var log = Devices.CreateLogDevice(Path.GetTempPath() + "hlog.log");
            var objlog = Devices.CreateLogDevice(Path.GetTempPath() + "hlog.obj.log");
            
            var h = new FasterKV
                <MyKey, MyValue, MyInput, MyOutput, MyContext, MyFunctions>
                (1L << 20, new MyFunctions(),
                new LogSettings { LogDevice = log, ObjectLogDevice = objlog, MemorySizeBits = 29,CopyReadsToTail = true},
                new CheckpointSettings(){CheckpointDir = ""},
                new SerializerSettings<MyKey, MyValue> { keySerializer = () => new MyKeySerializer(), valueSerializer = () => new MyValueSerializer() }
                );
            
            var context = default(MyContext);

            // Each thread calls StartSession to register itself with FASTER
            //var s = h.NewSession("Test");
            var g = Guid.Parse("6eb89566-8ef4-4280-8559-50be0cd3fbfe");
            h.Recover(g);
            var s = h.ResumeSession("Test", out var aa);
            
            MyKey key;

            key = new MyKey { Key = 23.ToString() };
            var inp1 = default(MyInput);
            MyOutput g11 = new MyOutput();
            var status1 = s.Read(ref key, ref inp1, ref g11, context, 0);


            for (int i = 0; i < 20000; i++)
            {
                key = new MyKey { Key = i.ToString() };
                var value = new MyValue
                {
                    Value = i,
                    ExpireDate = DateTime.Now.AddSeconds(new Random().Next(60))
                };
                s.Upsert(ref key, ref value, context, 0);

                // Each thread calls Refresh periodically for thread coordination
                if (i % 1024 == 0) s.Refresh();
            }

            h.Log.Flush(true);
            
            //h.TakeFullCheckpoint(out var aa);
            
            key = new MyKey { Key = 23.ToString() };
            var input = default(MyInput);
            MyOutput g1 = new MyOutput();
            var status = s.Read(ref key, ref input, ref g1, context, 0);

            if (status == Status.OK && g1.Value.Value.ToString() == key.Key)
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Error!");

            MyOutput g2 = new MyOutput();
            key = new MyKey { Key = 46.ToString() };
            status = s.Read(ref key, ref input, ref g2, context, 0);

            if (status == Status.OK && g2.Value.Value.ToString() == key.Key)
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Error!");

            var inp = new MyValue()
            {
                Value = 1000,
                ExpireDate = DateTime.Now.AddMinutes(1)
            };
            status = s.Upsert(ref key, ref inp, context, 0);

            var outp = new MyOutput();
            status = s.Read(ref key, ref input, ref outp, context, 0);

            // Delete key, read to verify deletion
            var output = new MyOutput();
            s.Delete(ref key, context, 0);
            status = s.Read(ref key, ref input, ref output, context, 0);
            if (status == Status.NOTFOUND)
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Error!");


            // Each thread ends session when done

            s.Dispose();

            // Dispose FASTER instance and log
            h.Dispose();
            log.Close();
            objlog.Close();

            Console.WriteLine("Press <ENTER> to end");
            Console.ReadLine();
        }

        private static void Test3()
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
                Id=100,
                Firstname = "Naser",
                Surname = "Rafinia",
                Age = age,
                Test = test
            };

            var ser= JsonSerializer.Serialize(person);
            var obj = JsonSerializer.Deserialize<Person>(ser);

        }
    }

    public class Base
    {
        public int Id { get; set; }
    }

    public class Person:Base
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
