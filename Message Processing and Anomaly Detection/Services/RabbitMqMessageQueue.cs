using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class RabbitMqMessageQueue : IMessageQueue, IDisposable
    {
        private IConnection Connection;
        private IModel Channel;

        public RabbitMqMessageQueue(string hostName, int port, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.ExchangeDeclare("server_stats_exchange", ExchangeType.Topic);
        }


        public void Subscribe(Action<ServerStatistics> MessageHandler)
        {
            Channel.QueueDeclare(queue: "Queue", durable: false, exclusive: false, autoDelete: false);


            Channel.QueueBind("Queue",
                              exchange: "server_stats_exchange",
                              routingKey: "ServerStatistics.*");

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
               
                var JsonMessage = Encoding.UTF8.GetString(body);
                var statistics = JsonSerializer.Deserialize<ServerStatistics>(JsonMessage);
                statistics.ServerIdentifier = args.RoutingKey.Substring("ServerStatistics.".Length);
                MessageHandler(statistics);

                Channel.BasicAck(args.DeliveryTag, multiple: false);
            };

            Channel.BasicConsume("Queue", autoAck: false, consumer);
        }
        public void Dispose()
        {
           
            Channel?.Dispose();
            Connection?.Dispose();
        
        }

       
    }
}
