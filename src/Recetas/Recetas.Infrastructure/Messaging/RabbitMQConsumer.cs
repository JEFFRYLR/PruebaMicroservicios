using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Recetas.Domain.Entities;
using Recetas.Infrastructure.Persistence;
using Recetas.Infrastructure.Repositories;
using System;
using System.Diagnostics;
using System.Text;

namespace Recetas.Infrastructure.Messaging
{
    /// <summary>
    /// Consumer simple de RabbitMQ - Escucha eventos de citas finalizadas
    /// Implementa patron de scope por mensaje para garantizar consistencia de datos
    /// </summary>
    public class RabbitMQConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "recetas.queue";

        public RabbitMQConsumer(string hostName = "168.231.74.78", string userName = "admin", string password = "admin123")
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

            // Declarar la cola con configuracion durable para persistencia
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Configurar QoS: procesar un mensaje a la vez para evitar sobrecarga
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Debug.WriteLine("[RabbitMQ Consumer] Conectado y escuchando cola '{0}' en {1}", _queueName, hostName);
            Trace.WriteLine(string.Format("[RabbitMQ Consumer] Conectado y escuchando cola '{0}' en {1}", _queueName, hostName));
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

                Debug.WriteLine("===========================================");
                Debug.WriteLine("[RabbitMQ Consumer] MENSAJE RECIBIDO");
                Debug.WriteLine("[RabbitMQ Consumer] Contenido: {0}", mensaje);
                Debug.WriteLine("===========================================");

                Trace.WriteLine("===========================================");
                Trace.WriteLine("[RabbitMQ Consumer] MENSAJE RECIBIDO");
                Trace.WriteLine(string.Format("[RabbitMQ Consumer] Contenido: {0}", mensaje));
                Trace.WriteLine("===========================================");

                // PATRON: Crear un nuevo scope de base de datos por cada mensaje
                RecetasDbContext context = null;
                
                try
                {
                    // Deserializar el mensaje
                    var evento = JsonConvert.DeserializeObject<CitaFinalizadaDto>(mensaje);

                    if (evento == null)
                    {
                        throw new InvalidOperationException("El mensaje no pudo ser deserializado a CitaFinalizadaDto");
                    }

                    Debug.WriteLine("[RabbitMQ Consumer] Evento deserializado correctamente");
                    Debug.WriteLine("  CitaId: {0}", evento.CitaId);
                    Debug.WriteLine("  PacienteId: {0}", evento.PacienteId);
                    Debug.WriteLine("  MedicoId: {0}", evento.MedicoId);
                    Debug.WriteLine("  Lugar: {0}", evento.Lugar);

                    Trace.WriteLine("[RabbitMQ Consumer] Evento deserializado correctamente");
                    Trace.WriteLine(string.Format("  CitaId: {0}", evento.CitaId));
                    Trace.WriteLine(string.Format("  PacienteId: {0}", evento.PacienteId));
                    Trace.WriteLine(string.Format("  MedicoId: {0}", evento.MedicoId));
                    Trace.WriteLine(string.Format("  Lugar: {0}", evento.Lugar));

                    // Crear contexto y repositorio con scope de mensaje
                    Debug.WriteLine("[RabbitMQ Consumer] Creando contexto de base de datos...");
                    Trace.WriteLine("[RabbitMQ Consumer] Creando contexto de base de datos...");
                    
                    context = new RecetasDbContext();
                    var repository = new RecetaRepository(context);

                    // Crear receta automaticamente con validaciones del dominio
                    Debug.WriteLine("[RabbitMQ Consumer] Creando entidad Receta...");
                    Trace.WriteLine("[RabbitMQ Consumer] Creando entidad Receta...");
                    
                    var receta = new Receta(
                        citaId: evento.CitaId,
                        medicoId: evento.MedicoId,
                        pacienteId: evento.PacienteId,
                        diagnostico: "Pendiente de ingreso",
                        vigencia: DateTime.Now.AddDays(30),
                        observaciones: string.Format("Receta generada automaticamente desde cita en {0}", evento.Lugar)
                    );

                    Debug.WriteLine("[RabbitMQ Consumer] Receta creada en memoria");
                    Trace.WriteLine("[RabbitMQ Consumer] Receta creada en memoria");

                    // Agregar y persistir en una sola transaccion
                    Debug.WriteLine("[RabbitMQ Consumer] Agregando receta al contexto...");
                    Trace.WriteLine("[RabbitMQ Consumer] Agregando receta al contexto...");
                    
                    repository.Agregar(receta);
                    
                    Debug.WriteLine("[RabbitMQ Consumer] Guardando cambios en base de datos...");
                    Trace.WriteLine("[RabbitMQ Consumer] Guardando cambios en base de datos...");
                    
                    repository.GuardarCambios();

                    Debug.WriteLine("[RabbitMQ Consumer] RECETA PERSISTIDA EXITOSAMENTE");
                    Debug.WriteLine("  CitaId: {0}", evento.CitaId);
                    
                    Trace.WriteLine("[RabbitMQ Consumer] RECETA PERSISTIDA EXITOSAMENTE");
                    Trace.WriteLine(string.Format("  CitaId: {0}", evento.CitaId));

                    // ACK: confirmar procesamiento exitoso SOLO despues de guardar en BD
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    
                    Debug.WriteLine("[RabbitMQ Consumer] ACK enviado a RabbitMQ");
                    Debug.WriteLine("===========================================");
                    
                    Trace.WriteLine("[RabbitMQ Consumer] ACK enviado a RabbitMQ");
                    Trace.WriteLine("===========================================");
                }
                catch (ArgumentException argEx)
                {
                    // Error de validacion de dominio
                    Debug.WriteLine("[ERROR DOMINIO] Validacion fallida: {0}", argEx.Message);
                    Debug.WriteLine("[ERROR] StackTrace: {0}", argEx.StackTrace);
                    
                    Trace.WriteLine(string.Format("[ERROR DOMINIO] Validacion fallida: {0}", argEx.Message));
                    Trace.WriteLine(string.Format("[ERROR] StackTrace: {0}", argEx.StackTrace));
                    
                    LogMensajeBruto(ea);
                    
                    // NACK sin requeue: el mensaje es invalido y no se debe reprocesar
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    
                    Debug.WriteLine("[RabbitMQ Consumer] NACK enviado (sin requeue) - mensaje invalido");
                    Trace.WriteLine("[RabbitMQ Consumer] NACK enviado (sin requeue) - mensaje invalido");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[ERROR] FALLO EN PROCESAMIENTO");
                    Debug.WriteLine("[ERROR] Mensaje: {0}", ex.Message);
                    Debug.WriteLine("[ERROR] Tipo: {0}", ex.GetType().FullName);
                    Debug.WriteLine("[ERROR] StackTrace: {0}", ex.StackTrace);

                    Trace.WriteLine("[ERROR] FALLO EN PROCESAMIENTO");
                    Trace.WriteLine(string.Format("[ERROR] Mensaje: {0}", ex.Message));
                    Trace.WriteLine(string.Format("[ERROR] Tipo: {0}", ex.GetType().FullName));
                    Trace.WriteLine(string.Format("[ERROR] StackTrace: {0}", ex.StackTrace));

                    var inner = ex.InnerException;
                    int nivel = 1;
                    while (inner != null)
                    {
                        Debug.WriteLine("[ERROR] InnerException (nivel {0}): {1}", nivel, inner.Message);
                        Debug.WriteLine("[ERROR] InnerException StackTrace: {0}", inner.StackTrace);
                        
                        Trace.WriteLine(string.Format("[ERROR] InnerException (nivel {0}): {1}", nivel, inner.Message));
                        Trace.WriteLine(string.Format("[ERROR] InnerException StackTrace: {0}", inner.StackTrace));
                        
                        inner = inner.InnerException;
                        nivel++;
                    }

                    LogMensajeBruto(ea);

                    // NACK con requeue=false para evitar loop infinito
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    
                    Debug.WriteLine("[RabbitMQ Consumer] NACK enviado (sin requeue)");
                    Debug.WriteLine("===========================================");
                    
                    Trace.WriteLine("[RabbitMQ Consumer] NACK enviado (sin requeue)");
                    Trace.WriteLine("===========================================");
                }
                finally
                {
                    // CRITICO: Disponer el contexto despues de procesar cada mensaje
                    if (context != null)
                    {
                        context.Dispose();
                        Debug.WriteLine("[RabbitMQ Consumer] Contexto de BD liberado");
                        Trace.WriteLine("[RabbitMQ Consumer] Contexto de BD liberado");
                    }
                }
            };

            // Iniciar consumo con ACK manual para control transaccional
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false, // ACK manual: solo confirmar despues de persistir
                consumer: consumer
            );

            Debug.WriteLine("[RabbitMQ Consumer] Consumo iniciado - Esperando mensajes...");
            Trace.WriteLine("[RabbitMQ Consumer] Consumo iniciado - Esperando mensajes...");
        }

        /// <summary>
        /// Helper para logging seguro del mensaje bruto
        /// </summary>
        private void LogMensajeBruto(BasicDeliverEventArgs ea)
        {
            try
            {
                var mensajeBruto = Encoding.UTF8.GetString(ea.Body.ToArray());
                Debug.WriteLine("[ERROR] Mensaje bruto: {0}", mensajeBruto);
                Trace.WriteLine(string.Format("[ERROR] Mensaje bruto: {0}", mensajeBruto));
            }
            catch (Exception logEx)
            {
                Debug.WriteLine("[ERROR] No se pudo loggear mensaje bruto: {0}", logEx.Message);
                Trace.WriteLine(string.Format("[ERROR] No se pudo loggear mensaje bruto: {0}", logEx.Message));
            }
        }

        public void Dispose()
        {
            try
            {
                Debug.WriteLine("[RabbitMQ Consumer] Cerrando conexion...");
                Trace.WriteLine("[RabbitMQ Consumer] Cerrando conexion...");
                
                _channel?.Close();
                _connection?.Close();
                
                Debug.WriteLine("[RabbitMQ Consumer] Conexion cerrada exitosamente");
                Trace.WriteLine("[RabbitMQ Consumer] Conexion cerrada exitosamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Error al cerrar RabbitMQ Consumer: {0}", ex.Message);
                Trace.WriteLine(string.Format("[ERROR] Error al cerrar RabbitMQ Consumer: {0}", ex.Message));
            }
        }
    }

    /// <summary>
    /// DTO para deserializar el evento de cita finalizada
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