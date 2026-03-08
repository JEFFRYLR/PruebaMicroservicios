using MediatR;
using Personas.Application.DTOs;
using System.Collections.Generic;

namespace Personas.Application.Queries
{
    /// <summary>
    /// Query para obtener todos los médicos
    /// </summary>
    public class ObtenerMedicosQuery : IRequest<IEnumerable<PersonaDto>>
    {
    }
}
