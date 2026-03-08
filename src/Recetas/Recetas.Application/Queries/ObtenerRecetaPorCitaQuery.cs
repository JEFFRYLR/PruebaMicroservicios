using MediatR;
using Recetas.Application.DTOs;

namespace Recetas.Application.Queries
{
    /// <summary>
    /// Query para obtener la receta de una cita específica
    /// </summary>
    public class ObtenerRecetaPorCitaQuery : IRequest<RecetaDto>
    {
        public int CitaId { get; set; }

        public ObtenerRecetaPorCitaQuery(int citaId)
        {
            CitaId = citaId;
        }
    }
}
