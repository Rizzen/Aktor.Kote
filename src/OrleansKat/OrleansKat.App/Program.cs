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
            var host = StartSilo();
            
            var client = await ConnectClient();
            var koteGrain = client.GetGrain<IKoteGrain>("Kote");
            
            try
            {
                await koteGrain.Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
//            for (int i = 0; i < 10000; i++)
//            {
//                var koteGrain = client.GetGrain<IKoteGrain>("Kote" + i);
//                await koteGrain.GetState();
//            }
            
            Console.WriteLine("\n\n Press Enter to terminate...\n\n");
            Console.ReadLine();
            
            await host.StopAsync();
        }
        
        private static async Task<IClusterClient> ConnectClient()
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansKote";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }
        
        private static ISiloHost StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering(siloPort: 11112)
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
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(KoteGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            _ = host.StartAsync();
            return host;
        }
    }
}