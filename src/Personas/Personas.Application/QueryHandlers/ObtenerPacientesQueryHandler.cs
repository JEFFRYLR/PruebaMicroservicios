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
    /// Handler para obtener todos los pacientes
    /// </summary>
    public class ObtenerPacientesQueryHandler : IRequestHandler<ObtenerPacientesQuery, IEnumerable<PersonaDto>>
    {
        private readonly IPersonaRepository _personaRepository;

        public ObtenerPacientesQueryHandler(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public Task<IEnumerable<PersonaDto>> Handle(ObtenerPacientesQuery request, CancellationToken cancellationToken)
        {
            var todasLasPersonas = _personaRepository.ObtenerTodos();
            var pacientes = todasLasPersonas.Where(p => p.TipoPersona == TipoPersona.Paciente);

            var resultado = pacientes.Select(p => new PersonaDto
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
