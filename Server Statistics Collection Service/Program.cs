using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Server_Statistics_Collection_Service.Interfaces;
using Server_Statistics_Collection_Service.Services;
using System;
namespace Server_Statistics_Collection_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                var rabbitMqHost = context.Configuration.GetValue<string>("RabbitMQConfig:HostName");
                var rabbitMqPort = context.Configuration.GetValue<int>("RabbitMQConfig:Port");
                var rabbitMqUser = context.Configuration.GetValue<string>("RabbitMQConfig:UserName");
                var rabbitMqPassword = context.Configuration.GetValue<string>("RabbitMQConfig:Password");
                if (string.IsNullOrEmpty(rabbitMqHost) || string.IsNullOrEmpty(rabbitMqUser) || string.IsNullOrEmpty(rabbitMqPassword))
                {
                    throw new ArgumentNullException("One of the RabbitMQ configuration values is null or empty.");
                }

                services.AddSingleton<IStatisticsPublisher>(provider => new RabbitMqMessageQueue(rabbitMqHost, rabbitMqPort, rabbitMqUser, rabbitMqPassword));
                services.AddHostedService<StatisticsCollectorService>();


            })
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();
                     logging.AddConsole();
                 })
                .Build();

            host.Run();
            
        }
    }
}