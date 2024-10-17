using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class RabbitMqMessageQueue : IMessageQueue
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

        public void Subscribe(Action<ServerStatistics> messageHandler)
        {

            _channel.ExchangeDeclare("ServerStatisticsExchange", ExchangeType.Topic);
            _channel.QueueDeclare("Queue", durable: false, exclusive: false, autoDelete: false);
            _channel.QueueBind("Queue", "ServerStatisticsExchange", routingKey: $"{"ServerStatistics."}*");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var statistics = JsonSerializer.Deserialize<ServerStatistics>(message);

                statistics.ServerIdentifier = args.RoutingKey.Substring("ServerStatistics.".Length);
                messageHandler(statistics);

                _channel.BasicAck(args.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume("Queue", autoAck: false, consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }

        
    }
}
