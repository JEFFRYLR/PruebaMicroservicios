using Citas.Application.DTOs;
using Citas.Application.Queries;
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
    /// Handler para obtener citas por paciente
    /// </summary>
    public class ObtenerCitasPorPacienteQueryHandler : IRequestHandler<ObtenerCitasPorPacienteQuery, List<CitaDto>>
    {
        private readonly ICitaRepository _repository;

        public ObtenerCitasPorPacienteQueryHandler(ICitaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<CitaDto>> Handle(ObtenerCitasPorPacienteQuery request, CancellationToken cancellationToken)
        {
            var citas = _repository.ObtenerPorPaciente(request.PacienteId)
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
