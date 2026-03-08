using MediatR;
using Personas.Application.DTOs;

namespace Personas.Application.Queries
{
    /// <summary>
    /// Query para obtener una persona por ID
    /// </summary>
    public class ObtenerPersonaPorIdQuery : IRequest<PersonaDto>
    {
        public int Id { get; set; }

        public ObtenerPersonaPorIdQuery(int id)
        {
            Id = id;
        }
    }
}
