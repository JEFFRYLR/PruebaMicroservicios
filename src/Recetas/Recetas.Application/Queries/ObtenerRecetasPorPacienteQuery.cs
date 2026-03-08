using MediatR;
using Recetas.Application.DTOs;
using System.Collections.Generic;

namespace Recetas.Application.Queries
{
    /// <summary>
    /// Query para obtener recetas de un paciente
    /// </summary>
    public class ObtenerRecetasPorPacienteQuery : IRequest<IEnumerable<RecetaDto>>
    {
        public int PacienteId { get; set; }

        public ObtenerRecetasPorPacienteQuery(int pacienteId)
        {
            PacienteId = pacienteId;
        }
    }
}
