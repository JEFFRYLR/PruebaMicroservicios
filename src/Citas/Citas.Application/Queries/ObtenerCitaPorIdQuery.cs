using Citas.Application.DTOs;
using MediatR;

namespace Citas.Application.Queries
{
    /// <summary>
    /// Query para obtener una cita por ID
    /// </summary>
    public class ObtenerCitaPorIdQuery : IRequest<CitaDto>
    {
        public int CitaId { get; set; }
    }
}
