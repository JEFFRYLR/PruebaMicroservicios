using Citas.Application.DTOs;
using Citas.Application.Queries;
using Citas.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Citas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener una cita por ID
    /// </summary>
    public class ObtenerCitaPorIdQueryHandler : IRequestHandler<ObtenerCitaPorIdQuery, CitaDto>
    {
        private readonly ICitaRepository _repository;

        public ObtenerCitaPorIdQueryHandler(ICitaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<CitaDto> Handle(ObtenerCitaPorIdQuery request, CancellationToken cancellationToken)
        {
            var cita = _repository.ObtenerPorId(request.CitaId);

            if (cita == null)
                return null;

            var dto = new CitaDto
            {
                Id = cita.Id,
                FechaCita = cita.FechaCita,
                Lugar = cita.Lugar,
                MedicoId = cita.MedicoId,
                PacienteId = cita.PacienteId,
                Estado = (int)cita.Estado,
                EstadoTexto = cita.Estado.ToString(),
                Motivo = cita.Motivo,
                FechaCreacion = cita.FechaCreacion,
                FechaActualizacion = cita.FechaActualizacion
            };

            return await Task.FromResult(dto);
        }
    }
}
