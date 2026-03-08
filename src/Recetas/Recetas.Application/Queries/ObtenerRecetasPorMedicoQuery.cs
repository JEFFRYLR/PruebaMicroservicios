using MediatR;
using Recetas.Application.DTOs;
using System.Collections.Generic;

namespace Recetas.Application.Queries
{
    /// <summary>
    /// Query para obtener recetas emitidas por un médico
    /// </summary>
    public class ObtenerRecetasPorMedicoQuery : IRequest<IEnumerable<RecetaDto>>
    {
        public int MedicoId { get; set; }

        public ObtenerRecetasPorMedicoQuery(int medicoId)
        {
            MedicoId = medicoId;
        }
    }
}
