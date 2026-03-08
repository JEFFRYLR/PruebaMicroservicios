using MediatR;
using Personas.Application.Commands;
using Personas.Domain.Entities;
using Personas.Domain.Interfaces;
using Personas.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para crear una nueva persona
    /// </summary>
    public class CrearPersonaCommandHandler : IRequestHandler<CrearPersonaCommand, int>
    {
        private readonly IPersonaRepository _personaRepository;

        public CrearPersonaCommandHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<int> Handle(CrearPersonaCommand request, CancellationToken cancellationToken)
        {
            var documento = new Documento(request.TipoDocumento, request.NumeroDocumento);
            var persona = new Persona(request.Nombre, request.Apellido, documento, request.TipoPersona);

            _personaRepository.Crear(persona);

            return Task.FromResult(persona.Id);
        }
    }
}
