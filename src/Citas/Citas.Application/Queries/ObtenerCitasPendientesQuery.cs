using Citas.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Citas.Application.Queries
{
    /// <summary>
    /// Query para obtener todas las citas pendientes
    /// </summary>
    public class ObtenerCitasPendientesQuery : IRequest<List<CitaDto>>
    {
    }
}
