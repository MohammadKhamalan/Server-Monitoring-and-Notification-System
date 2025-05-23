using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

                    services.AddSingleton<IAnomalyDetectionService, AnomalyDetectionService>();
                    services.AddSingleton<ISignalRService, SignalRService>(provider =>
                    {
                        var signalRUrl = configuration.GetValue<string>("SignalRConfig:SignalRUrl");
                        return new SignalRService(signalRUrl);
                    });

                    services.AddSingleton<IMongoDbService, MongoDbService>(provider =>
                    {
                        var connectionString = configuration.GetValue<string>("MongoDb:ConnectionString");
                        var databaseName = configuration.GetValue<string>("MongoDb:DatabaseName");
                        var collectionName = configuration.GetValue<string>("MongoDb:CollectionName");
                        return new MongoDbService(connectionString, databaseName, collectionName);
                    });

                    services.AddSingleton<IMessageQueue, RabbitMqMessageQueue>(provider =>
                    {
                        var hostName = configuration.GetValue<string>("RabbitMQConfig:HostName");
                        var port = configuration.GetValue<int>("RabbitMQConfig:Port");
                        var userName = configuration.GetValue<string>("RabbitMQConfig:UserName");
                        var password = configuration.GetValue<string>("RabbitMQConfig:Password");
                        return new RabbitMqMessageQueue(hostName, port, userName, password);
                    });

                    services.AddSingleton<StatisticsProcessor>();
                })
                .Build();

            var messageQueue = host.Services.GetRequiredService<IMessageQueue>();
            var statisticsProcessor = host.Services.GetRequiredService<StatisticsProcessor>();

            messageQueue.Subscribe(async (statistics) =>
            {
                await statisticsProcessor.ProcessStatistics(statistics);
            });

            await host.RunAsync();
        }
    }
}