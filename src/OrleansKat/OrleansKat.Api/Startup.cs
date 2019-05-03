using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansKat.Api.Infrastructure;
using OrleansKat.Core;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace OrleansKat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var silo = StartSilo();
            var client = ConnectClient().Result;
            
            services.AddSingleton(silo);
            services.AddSingleton(client);
            
            services.RegisterSwaggerGenerator(Configuration)
                    .AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc()
               .UseSwaggerAndReDoc(Configuration);
        }
        
        private static ISiloHost StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering(siloPort: 11112)
                .Configure<GrainCollectionOptions>(options =>
                {
                    options.CollectionAge = new TimeSpan(0, 0, 15);
                    options.CollectionQuantum = new TimeSpan(0, 0, 5);
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansKote";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(KoteGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            _ = host.StartAsync();
            return host;
        }
        
        private static async Task<IClusterClient> ConnectClient()
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<GrainCollectionOptions>(options =>
                {
                    options.CollectionAge = new TimeSpan(0, 0, 10);
                    options.CollectionQuantum = new TimeSpan(0, 0, 1);
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansKote";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            return client;
        }
    }
}