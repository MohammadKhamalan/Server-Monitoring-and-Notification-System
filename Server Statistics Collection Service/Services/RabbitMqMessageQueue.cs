using RabbitMQ.Client;

using Server_Statistics_Collection_Service.Interfaces;
using Server_Statistics_Collection_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server_Statistics_Collection_Service.Services
{
    public class RabbitMqMessageQueue : IStatisticsPublisher, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        public RabbitMqMessageQueue(string hostname, int port, string UserName, string Paswword)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostname,
                Port = port,
                UserName = UserName,
                Password = Paswword

            };
            factory.ClientProvidedName= "Server Statistics Collector Service";
            _connection= factory.CreateConnection();
            _channel = _connection.CreateModel();

        }
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }

        public async Task PublishAsync(ServerStatistics statistics, string topic)
        {
            _channel.ExchangeDeclare("ServerStatisticsExchange", ExchangeType.Topic);
            _channel.QueueDeclare("Queue", durable: false, exclusive: false, autoDelete: false);
            _channel.QueueBind("Queue", "ServerStatisticsExchange", topic);
            string MeassageInJson = JsonSerializer.Serialize(statistics);
            var body = Encoding.UTF8.GetBytes(MeassageInJson);
            await Task.Run(() => _channel.BasicPublish(
                exchange: "ServerStatisticsExchange",
                routingKey: topic,
                basicProperties: null,
                body: body
                ));
        }
    }
}
