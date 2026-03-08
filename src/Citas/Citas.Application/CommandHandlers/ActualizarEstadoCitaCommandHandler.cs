using Citas.Application.Commands;
using Citas.Domain.Enums;
using Citas.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Citas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para actualizar el estado de una cita
    /// </summary>
    public class ActualizarEstadoCitaCommandHandler : IRequestHandler<ActualizarEstadoCitaCommand, Unit>
    {
        private readonly ICitaRepository _repository;

        public ActualizarEstadoCitaCommandHandler(ICitaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Unit> Handle(ActualizarEstadoCitaCommand request, CancellationToken cancellationToken)
        {
            var cita = _repository.ObtenerPorId(request.CitaId);

            if (cita == null)
                throw new InvalidOperationException($"No se encontró la cita con ID {request.CitaId}");

            var nuevoEstado = (EstadoCita)request.NuevoEstado;
            cita.ActualizarEstado(nuevoEstado);

            _repository.Actualizar(cita);
            _repository.GuardarCambios();

            return await Task.FromResult(Unit.Value);
        }
    }
}
