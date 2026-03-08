using MediatR;
using Personas.Application.DTOs;
using System.Collections.Generic;

namespace Personas.Application.Queries
{
    /// <summary>
    /// Query para obtener todas las personas
    /// </summary>
    public class ObtenerTodasPersonasQuery : IRequest<IEnumerable<PersonaDto>>
    {
    }
}
