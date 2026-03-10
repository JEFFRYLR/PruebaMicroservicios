using Citas.Application.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Citas.Infrastructure.Messaging
{
    /// <summary>
    /// Publisher simple de RabbitMQ usando cola directa
    /// </summary>
    public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQPublisher(string hostName = "168.231.74.78", string userName = "admin", string password = "admin123")
        {
            var factory = new ConnectionFactory() 
            { 
                HostName = hostName,
                Port = 5672,
                UserName = userName,
                Password = password
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declarar cola directamente
            _channel.QueueDeclare(
                queue: "recetas.queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            Console.WriteLine("[RabbitMQ Publisher] Conectado a RabbitMQ en {0}", hostName);
        }

        public void PublicarMensaje(string queueName, object message)
        {
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            // Publicar directamente en la cola
            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine("[RabbitMQ Publisher] Mensaje enviado a cola '{0}'", queueName);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
