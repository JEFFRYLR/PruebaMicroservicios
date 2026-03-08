using Citas.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Citas.Application.Queries
{
    /// <summary>
    /// Query para obtener citas de un paciente específico
    /// </summary>
    public class ObtenerCitasPorPacienteQuery : IRequest<List<CitaDto>>
    {
        public int PacienteId { get; set; }
    }
}
