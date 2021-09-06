using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using TriteUtilities.Azure.Blob;

namespace TrashPandaBot
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false);
                    var config = builder.Build();

                    //services.Configure<BlobStorageOptions>(config.GetSection(BlobStorageOptions.configLocation));

                    //services.AddSingleton<BlobStorageService>();

                    services.RegisterStorageService(config);

                    services.AddHostedService<Worker>();
                });
    }
}
