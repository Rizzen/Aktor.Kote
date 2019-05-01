using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansKat.Core;

namespace OrleansKat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = await StartSilo();
            Console.WriteLine("\n\n Press Enter to terminate...\n\n");
            Console.ReadLine();

            await host.StopAsync();
        }
        
        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansKote";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(KoteGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}