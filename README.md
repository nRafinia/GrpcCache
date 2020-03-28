
# nCache
  Small and lightweight cache memory service for use in distributed or separate systems with the ability to separate information from each system

# Install Service:
  + Clone project
  + Compile code
  + Add service to IIS or install as service (With NSSM)
  + If use as service config port number
    > "Port": "37532"

# Use in Clients:
  + Add nuget
    > Install-Package nCache.Client
  + Add Service url in AppSettings.json    
    >   "CacheMemory": "http://localhost:37532"
    
# Example

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
    
    var person = new Person()
    {
        Firstname = "Naser",
        Surname = "Rafinia",
        Age = 35,
        Test = new P2()
        {
            Name = "Test"
        }
    };

    var cache = CacheMemory.GetInstance("http://localhost:37532/");
    cache.AddOrUpdate("TestModel", person);
    var p = cache.Get<Person>("TestModel");
    Console.Write($"Age is equal={person.Age == p?.Age}");
    
