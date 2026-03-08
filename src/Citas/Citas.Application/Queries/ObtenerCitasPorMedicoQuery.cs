using Citas.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Citas.Application.Queries
{
    /// <summary>
    /// Query para obtener citas de un médico específico
    /// </summary>
    public class ObtenerCitasPorMedicoQuery : IRequest<List<CitaDto>>
    {
        public int MedicoId { get; set; }
    }
}
