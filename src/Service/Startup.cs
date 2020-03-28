using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Server;
using nCache.Service.Providers;

namespace nCache.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCodeFirstGrpc(config =>
            {
                //config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment _)
        {
            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints  =>
            {
                endpoints.MapGrpcService<CacheMemoryContract>();
                endpoints.MapControllers();
            });

        }
    }
}