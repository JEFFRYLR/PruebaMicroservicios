using Citas.Application.Commands;
using Citas.Domain.Entities;
using Citas.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Citas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para agendar una nueva cita
    /// </summary>
    public class AgendarCitaCommandHandler : IRequestHandler<AgendarCitaCommand, int>
    {
        private readonly ICitaRepository _repository;

        public AgendarCitaCommandHandler(ICitaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<int> Handle(AgendarCitaCommand request, CancellationToken cancellationToken)
        {
            var cita = new Cita(
                request.FechaCita,
                request.Lugar,
                request.MedicoId,
                request.PacienteId,
                request.Motivo
            );

            _repository.Crear(cita);
            _repository.GuardarCambios();

            return await Task.FromResult(cita.Id);
        }
    }
}
