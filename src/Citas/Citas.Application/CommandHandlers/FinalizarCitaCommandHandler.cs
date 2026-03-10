using Citas.Application.Commands;
using Citas.Application.Interfaces;
using Citas.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Citas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para finalizar una cita y emitir evento a RabbitMQ
    /// </summary>
    public class FinalizarCitaCommandHandler : IRequestHandler<FinalizarCitaCommand, Unit>
    {
        private readonly ICitaRepository _repository;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;

        public FinalizarCitaCommandHandler(
            ICitaRepository repository,
            IRabbitMQPublisher rabbitMQPublisher)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _rabbitMQPublisher = rabbitMQPublisher ?? throw new ArgumentNullException(nameof(rabbitMQPublisher));
        }

        public async Task<Unit> Handle(FinalizarCitaCommand request, CancellationToken cancellationToken)
        {
            var cita = _repository.ObtenerPorId(request.CitaId);

            if (cita == null)
                throw new InvalidOperationException($"No se encontró la cita con ID {request.CitaId}");

            cita.Finalizar();
            _repository.Actualizar(cita);
            _repository.GuardarCambios();

            // Mensaje con todos los datos requeridos
            var mensajeReceta = new
            {
                CitaId = cita.Id,
                MedicoId = cita.MedicoId,
                PacienteId = cita.PacienteId,
                FechaCita = cita.FechaCita,
                Lugar = cita.Lugar  // ✅ AGREGAR ESTE CAMPO
            };

            _rabbitMQPublisher.PublicarMensaje("recetas.queue", mensajeReceta);

            return await Task.FromResult(Unit.Value);
        }
    }
}
