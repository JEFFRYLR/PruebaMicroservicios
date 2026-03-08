using MediatR;
using Personas.Application.DTOs;
using Personas.Application.Queries;
using Personas.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener todas las personas
    /// </summary>
    public class ObtenerTodasPersonasQueryHandler : IRequestHandler<ObtenerTodasPersonasQuery, IEnumerable<PersonaDto>>
    {
        private readonly IPersonaRepository _personaRepository;

        public ObtenerTodasPersonasQueryHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<IEnumerable<PersonaDto>> Handle(ObtenerTodasPersonasQuery request, CancellationToken cancellationToken)
        {
            var personas = _personaRepository.ObtenerTodos();

            var resultado = personas.Select(p => new PersonaDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                NumeroDocumento = p.Documento.Numero,
                TipoDocumento = p.Documento.TipoDocumento,
                TipoPersona = p.TipoPersona
            });

            return Task.FromResult(resultado);
        }
    }
}
