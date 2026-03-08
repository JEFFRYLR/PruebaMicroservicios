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
    /// Handler para obtener citas por médico
    /// </summary>
    public class ObtenerCitasPorMedicoQueryHandler : IRequestHandler<ObtenerCitasPorMedicoQuery, List<CitaDto>>
    {
        private readonly ICitaRepository _repository;

        public ObtenerCitasPorMedicoQueryHandler(ICitaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<CitaDto>> Handle(ObtenerCitasPorMedicoQuery request, CancellationToken cancellationToken)
        {
            var citas = _repository.ObtenerPorMedico(request.MedicoId)
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
