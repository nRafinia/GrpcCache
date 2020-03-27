using System;
using System.Threading.Tasks;
using F4ST.Common.Extensions;
using F4ST.Common.Tools;
using Grpc.Net.Client;
using Newtonsoft.Json;
using ProtoBuf.Grpc.Client;
using RestSharp;
using Test.Models;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var m = new TestModel()
            {
                Id = 10,
                Name = "Test"
            };

            var data = new AddOrUpdateModel()
            {
                Key = "Test_Object",
                Item = JsonConvert.SerializeObject(m).Base64Encode(),
                ExpireDate = DateTime.Now.AddHours(1),
                Provider = 1
            };

            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            using (var channel = GrpcChannel.ForAddress("http://127.0.0.1:37532"))
            {
                var cache = channel.CreateGrpcService<ICacheMemoryContract>();
                cache.AddOrUpdate(data);
                var rData = cache.Get(new BaseModel()
                {
                    Key = "Test_Object",
                    Provider = 1
                });
                /*cData = JsonConvert.DeserializeObject<TestModel>(rData.Data.Base64Decode());
                Console.WriteLine(JsonConvert.SerializeObject(cData, Formatting.Indented));*/
            }
            
            Console.ReadKey();
        }
    }
}
