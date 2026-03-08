using MediatR;

namespace Citas.Application.Commands
{
    /// <summary>
    /// Command para actualizar el estado de una cita
    /// </summary>
    public class ActualizarEstadoCitaCommand : IRequest<Unit>
    {
        public int CitaId { get; set; }
        public int NuevoEstado { get; set; }
    }
}
