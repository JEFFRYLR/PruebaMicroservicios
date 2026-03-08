using MediatR;
using Personas.Application.DTOs;
using System.Collections.Generic;

namespace Personas.Application.Queries
{
    /// <summary>
    /// Query para obtener todos los pacientes
    /// </summary>
    public class ObtenerPacientesQuery : IRequest<IEnumerable<PersonaDto>>
    {
    }
}
