using MediatR;
using Recetas.Application.DTOs;
using Recetas.Application.Queries;
using Recetas.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Recetas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener receta por cita
    /// </summary>
    public class ObtenerRecetaPorCitaQueryHandler : IRequestHandler<ObtenerRecetaPorCitaQuery, RecetaDto>
    {
        private readonly IRecetaRepository _repository;

        public ObtenerRecetaPorCitaQueryHandler(IRecetaRepository repository)
        {
            _repository = repository;
        }

        public Task<RecetaDto> Handle(ObtenerRecetaPorCitaQuery request, CancellationToken cancellationToken)
        {
            var receta = _repository.ObtenerPorCita(request.CitaId);

            if (receta == null)
                return Task.FromResult<RecetaDto>(null);

            var dto = new RecetaDto
            {
                Id = receta.Id,
                CitaId = receta.CitaId,
                MedicoId = receta.MedicoId,
                PacienteId = receta.PacienteId,
                Diagnostico = receta.Diagnostico,
                FechaEmision = receta.FechaEmision,
                Observaciones = receta.Observaciones,
                Vigencia = receta.Vigencia,
                Detalles = receta.Detalles.Select(d => new DetalleRecetaDto
                {
                    Id = d.Id,
                    RecetaId = d.RecetaId,
                    NombreMedicamento = d.NombreMedicamento,
                    Dosis = d.Dosis,
                    Frecuencia = d.Frecuencia,
                    Duracion = d.Duracion,
                    Indicaciones = d.Indicaciones
                }).ToList()
            };

            return Task.FromResult(dto);
        }
    }
}
