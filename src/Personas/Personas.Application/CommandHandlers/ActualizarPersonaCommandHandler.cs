using MediatR;
using Personas.Application.Commands;
using Personas.Domain.Interfaces;
using Personas.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para actualizar una persona existente
    /// </summary>
    public class ActualizarPersonaCommandHandler : IRequestHandler<ActualizarPersonaCommand, Unit>
    {
        private readonly IPersonaRepository _personaRepository;

        public ActualizarPersonaCommandHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<Unit> Handle(ActualizarPersonaCommand request, CancellationToken cancellationToken)
        {
            var persona = _personaRepository.ObtenerPorId(request.Id);

            if (persona == null)
                throw new Exception($"Persona con ID {request.Id} no encontrada");

            var documento = new Documento(request.TipoDocumento, request.NumeroDocumento);
            persona.Actualizar(request.Nombre, request.Apellido, documento, persona.TipoPersona);

            _personaRepository.Actualizar(persona);

            return Task.FromResult(Unit.Value);
        }
    }
}
