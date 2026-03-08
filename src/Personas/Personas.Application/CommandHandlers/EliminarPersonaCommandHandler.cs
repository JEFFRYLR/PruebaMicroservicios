using MediatR;
using Personas.Application.Commands;
using Personas.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para eliminar una persona
    /// </summary>
    public class EliminarPersonaCommandHandler : IRequestHandler<EliminarPersonaCommand, Unit>
    {
        private readonly IPersonaRepository _personaRepository;

        public EliminarPersonaCommandHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<Unit> Handle(EliminarPersonaCommand request, CancellationToken cancellationToken)
        {
            var persona = _personaRepository.ObtenerPorId(request.Id);

            if (persona == null)
                throw new Exception($"Persona con ID {request.Id} no encontrada");

            _personaRepository.Eliminar(request.Id);

            return Task.FromResult(Unit.Value);
        }
    }
}
