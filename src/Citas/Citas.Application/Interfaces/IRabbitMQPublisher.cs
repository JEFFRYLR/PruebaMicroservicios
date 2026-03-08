using System;

namespace Citas.Application.Interfaces
{
    /// <summary>
    /// Contrato para publicar mensajes en RabbitMQ
    /// </summary>
    public interface IRabbitMQPublisher : IDisposable
    {
        /// <summary>
        /// Publica un mensaje a una cola específica
        /// </summary>
        /// <param name="queueName">Nombre de la cola</param>
        /// <param name="message">Mensaje a publicar (objeto)</param>
        void PublicarMensaje(string queueName, object message);
    }
}
