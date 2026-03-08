using MediatR;
using Personas.Application.DTOs;
using Personas.Application.Queries;
using Personas.Domain.Enums;
using Personas.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Personas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener todos los médicos
    /// </summary>
    public class ObtenerMedicosQueryHandler : IRequestHandler<ObtenerMedicosQuery, IEnumerable<PersonaDto>>
    {
        private readonly IPersonaRepository _personaRepository;

        public ObtenerMedicosQueryHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<IEnumerable<PersonaDto>> Handle(ObtenerMedicosQuery request, CancellationToken cancellationToken)
        {
            var todasLasPersonas = _personaRepository.ObtenerTodos();
            var medicos = todasLasPersonas.Where(p => p.TipoPersona == TipoPersona.Medico);

            var resultado = medicos.Select(m => new PersonaDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Apellido = m.Apellido,
                NumeroDocumento = m.Documento.Numero,
                TipoDocumento = m.Documento.TipoDocumento,
                TipoPersona = m.TipoPersona
            });

            return Task.FromResult(resultado);
        }
    }
}
