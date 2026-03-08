using MediatR;
using Recetas.Application.DTOs;

namespace Recetas.Application.Queries
{
    /// <summary>
    /// Query para obtener una receta por ID (con detalles de medicamentos)
    /// </summary>
    public class ObtenerRecetaPorIdQuery : IRequest<RecetaDto>
    {
        public int Id { get; set; }

        public ObtenerRecetaPorIdQuery(int id)
        {
            Id = id;
        }
    }
}
