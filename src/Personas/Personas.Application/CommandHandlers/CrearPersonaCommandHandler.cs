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
    ///  Implementa IRequestHandler<CrearPersonaCommand, int>
    /// </summary>
    public class CrearPersonaCommandHandler : IRequestHandler<CrearPersonaCommand, int>
    {
        private readonly IPersonaRepository _personaRepository;

        //  Unity inyecta IPersonaRepository automáticamente
        public CrearPersonaCommandHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<int> Handle(CrearPersonaCommand request, CancellationToken cancellationToken)
        {
            // Crear Value Object
            var documento = new Documento(request.TipoDocumento, request.NumeroDocumento);

            // Crear Entidad de Dominio (con validaciones)
            var persona = new Persona(
                request.Nombre,
                request.Apellido,
                documento,
                request.TipoPersona
            );

            // Persistir en BD
            _personaRepository.Crear(persona);

            // Retornar ID generado
            return Task.FromResult(persona.Id);
        }
    }
}
