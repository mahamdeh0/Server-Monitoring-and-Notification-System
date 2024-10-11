using RabbitMQ.Client;
using Server_Statistics_Collection_Service.Interfaces;
using Server_Statistics_Collection_Service.Models;
using System.Text;
using System.Text.Json;

namespace Server_Statistics_Collection_Service.Services
{
    public class RabbitMqMessageQueue : IMessageQueue, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqMessageQueue(string hostName, int port, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("ServerStatisticsExchange", ExchangeType.Topic);
        }

        public void Publish(string topic, ServerStatistics statistics)
        {
            string jsonMessage = JsonSerializer.Serialize(statistics);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(
                exchange: "ServerStatisticsExchange",
                routingKey: topic,
                basicProperties: null,
                body: body
            );
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
