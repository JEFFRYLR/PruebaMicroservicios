using Citas.Application.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace Citas.Infrastructure.Messaging
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _hostName;

        public RabbitMQPublisher(string hostName = "168.231.74.78", string userName = "admin", string password = "admin123")
        {
            try
            {
                _hostName = hostName;
                Debug.WriteLine($"[RabbitMQ Publisher] Intentando conectar a {hostName}:5672");
                
                var factory = new ConnectionFactory() 
                { 
                    HostName = hostName,
                    Port = 5672,
                    UserName = userName,
                    Password = password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(60), //  Agregar heartbeat
                    AutomaticRecoveryEnabled = true //  Reconexión automática
                };
                
                _connection = factory.CreateConnection();
                Debug.WriteLine($" Conexión establecida a RabbitMQ");
                
                _channel = _connection.CreateModel();
                Debug.WriteLine($" Canal creado");

                // Declarar cola directamente
                _channel.QueueDeclare(
                    queue: "recetas.queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Console.WriteLine($" [RabbitMQ Publisher] Conectado a RabbitMQ en {hostName}");
                Debug.WriteLine($" Cola 'recetas.queue' declarada correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" ERROR CRITICO EN RABBITMQ PUBLISHER:");
                Debug.WriteLine($"   Mensaje: {ex.Message}");
                Debug.WriteLine($"   Tipo: {ex.GetType().Name}");
                Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"   InnerException: {ex.InnerException.Message}");
                }
                
                throw; //  RE-LANZAR para que DependencyConfig lo capture
            }
        }

        public void PublicarMensaje(string queueName, object message)
        {
            try
            {
                var messageJson = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                Debug.WriteLine($" [RabbitMQ Publisher] Preparando mensaje...");
                Debug.WriteLine($"   Cola: {queueName}");
                Debug.WriteLine($"   Payload: {messageJson}");

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                // Publicar directamente en la cola
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body
                );

                Console.WriteLine($" [RabbitMQ Publisher] Mensaje enviado a cola '{queueName}'");
                Debug.WriteLine($" Mensaje publicado exitosamente en {_hostName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" ERROR AL PUBLICAR MENSAJE:");
                Debug.WriteLine($"   {ex.Message}");
                Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                Debug.WriteLine("[RabbitMQ Publisher] Cerrando conexión...");
                _channel?.Close();
                _connection?.Close();
                Debug.WriteLine(" Conexión RabbitMQ cerrada correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Error al cerrar conexión: {ex.Message}");
            }
        }
    }
}
