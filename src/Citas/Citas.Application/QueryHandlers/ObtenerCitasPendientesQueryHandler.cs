using Citas.Application.DTOs;
using Citas.Application.Queries;
using Citas.Domain.Enums;
using Citas.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Citas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener todas las citas pendientes
    /// </summary>
    public class ObtenerCitasPendientesQueryHandler : IRequestHandler<ObtenerCitasPendientesQuery, List<CitaDto>>
    {
        private readonly ICitaRepository _repository;

        public ObtenerCitasPendientesQueryHandler(ICitaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<CitaDto>> Handle(ObtenerCitasPendientesQuery request, CancellationToken cancellationToken)
        {
            var citas = _repository.ObtenerPorEstado(EstadoCita.Pendiente)
                .Select(c => new CitaDto
                {
                    Id = c.Id,
                    FechaCita = c.FechaCita,
                    Lugar = c.Lugar,
                    MedicoId = c.MedicoId,
                    PacienteId = c.PacienteId,
                    Estado = (int)c.Estado,
                    EstadoTexto = c.Estado.ToString(),
                    Motivo = c.Motivo,
                    FechaCreacion = c.FechaCreacion,
                    FechaActualizacion = c.FechaActualizacion
                })
                .ToList();

            return await Task.FromResult(citas);
        }
    }
}
