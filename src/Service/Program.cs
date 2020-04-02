using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace GrpcCache.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var port = 0;
            if (!InsideIIS)
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json");
                var config = builder.Build();
                port = Convert.ToInt32(config["Port"] ?? "37532");
            }

            CreateHostBuilder(args, port).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args, int port)
        {

            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureKestrel(options =>
                {
                    if (!InsideIIS)
                    {
                        options.ListenAnyIP(port, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                    }
                })
                .UseStartup<Startup>();
        }

        private static bool InsideIIS =>
            System.Environment.GetEnvironmentVariable("APP_POOL_ID") != null;
    }
}