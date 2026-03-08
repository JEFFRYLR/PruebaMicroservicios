using MediatR;
using Personas.Application.DTOs;
using Personas.Application.Queries;
using Personas.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener una persona por ID
    /// </summary>
    public class ObtenerPersonaPorIdQueryHandler : IRequestHandler<ObtenerPersonaPorIdQuery, PersonaDto>
    {
        private readonly IPersonaRepository _personaRepository;

        public ObtenerPersonaPorIdQueryHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<PersonaDto> Handle(ObtenerPersonaPorIdQuery request, CancellationToken cancellationToken)
        {
            var persona = _personaRepository.ObtenerPorId(request.Id);

            if (persona == null)
                return Task.FromResult<PersonaDto>(null);

            var resultado = new PersonaDto
            {
                Id = persona.Id,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                NumeroDocumento = persona.Documento.Numero,
                TipoDocumento = persona.Documento.TipoDocumento,
                TipoPersona = persona.TipoPersona
            };

            return Task.FromResult(resultado);
        }
    }
}
