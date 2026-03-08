using MediatR;

namespace Citas.Application.Commands
{
    /// <summary>
    /// Command para finalizar una cita y emitir evento a RabbitMQ para crear receta
    /// </summary>
    public class FinalizarCitaCommand : IRequest<Unit>
    {
        public int CitaId { get; set; }
    }
}
