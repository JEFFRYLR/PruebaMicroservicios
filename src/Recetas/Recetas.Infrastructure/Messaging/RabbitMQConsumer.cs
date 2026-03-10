using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Recetas.Domain.Entities;
using Recetas.Domain.Interfaces;
using System;
using System.Text;

namespace Recetas.Infrastructure.Messaging
{
    /// <summary>
    /// Consumer simple de RabbitMQ - Escucha eventos de citas finalizadas
    /// </summary>
    public class RabbitMQConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IRecetaRepository _repository;
        private readonly string _queueName = "recetas.queue";

        public RabbitMQConsumer(IRecetaRepository repository, string hostName = "168.231.74.78", string userName = "admin", string password = "admin123")
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            var factory = new ConnectionFactory() 
            { 
                HostName = hostName,
                Port = 5672,
                UserName = userName,
                Password = password
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Solo declarar la cola
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            Console.WriteLine("[RabbitMQ Consumer] Conectado y escuchando cola '{0}' en {1}", _queueName, hostName);
        }

        /// <summary>
        /// Iniciar escucha de mensajes
        /// </summary>
        public void IniciarEscucha()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var mensaje = Encoding.UTF8.GetString(body);

                Console.WriteLine("[RabbitMQ Consumer] Mensaje recibido: {0}", mensaje);

                try
                {
                    // Deserializar el mensaje
                    var evento = JsonConvert.DeserializeObject<CitaFinalizadaDto>(mensaje);

                    // Crear receta automáticamente
                    var receta = new Receta(
                        citaId: evento.CitaId,
                        medicoId: evento.MedicoId,
                        pacienteId: evento.PacienteId,
                        diagnostico: "Pendiente de ingreso",
                        vigencia: DateTime.Now.AddDays(30),
                        observaciones: string.Format("Receta generada automaticamente desde cita en {0}", evento.Lugar)
                    );

                    _repository.Agregar(receta);
                    _repository.GuardarCambios();

                    Console.WriteLine("[RabbitMQ Consumer] Receta creada para CitaId: {0}", evento.CitaId);

                    // ACK: confirmar procesamiento exitoso
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Fallo procesamiento: {0}", ex.Message);
                    
                    // NACK: rechazar mensaje
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            // Iniciar consumo
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false, // ACK manual
                consumer: consumer
            );
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }

    /// <summary>
    /// DTO para deserializar el evento
    /// </summary>
    public class CitaFinalizadaDto
    {
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public DateTime FechaCita { get; set; }
        public string Lugar { get; set; }
    }
}