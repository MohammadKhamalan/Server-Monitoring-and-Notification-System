using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Message_Processing_and_Anomaly_Detection.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization.Serializers;

namespace Message_Processing_and_Anomaly_Detection
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddSingleton<MongoDbService>();

                    services.AddSingleton<IMessageQueue, RabbitMqMessageQueue>(provider =>
                    {
                        var hostName = configuration.GetValue<string>("RabbitMQConfig:HostName");
                        var port = configuration.GetValue<int>("RabbitMQConfig:Port");
                        var userName = configuration.GetValue<string>("RabbitMQConfig:UserName");
                        var password = configuration.GetValue<string>("RabbitMQConfig:Password");
                        return new RabbitMqMessageQueue(hostName, port, userName, password);
                    });
                })
                    .Build();

            var messageQueue = host.Services.GetRequiredService<IMessageQueue>();
            messageQueue.Subscribe(HandleServerStatistics);

            var mongoService = host.Services.GetRequiredService<MongoDbService>();
            messageQueue.Subscribe(stats =>
            {
                HandleServerStatistics(stats);
                mongoService.InsertAsync(stats).Wait();
            });
            Console.WriteLine("Subscribed to RabbitMQ messages. Press Enter to exit.");
            Console.ReadLine();

            if (messageQueue is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private static void HandleServerStatistics(ServerStatistics stats)
        {
            Console.WriteLine("Received Server Statistics:");
            Console.WriteLine($"  Server ID      : {stats.ServerIdentifier}");
            Console.WriteLine($"  CPU Usage      : {stats.CpuUsage}%");
            Console.WriteLine($"  Memory Usage   : {stats.MemoryUsage} MB");
            Console.WriteLine($"  Available Mem  : {stats.AvailableMemory} MB");
            Console.WriteLine($"  Timestamp      : {stats.Timestamp}");
            Console.WriteLine(new string('-', 40));
        }
    }
}