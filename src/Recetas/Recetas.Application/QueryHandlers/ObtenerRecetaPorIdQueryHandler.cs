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
    /// Handler para obtener receta por ID
    /// </summary>
    public class ObtenerRecetaPorIdQueryHandler : IRequestHandler<ObtenerRecetaPorIdQuery, RecetaDto>
    {
        private readonly IRecetaRepository _repository;

        public ObtenerRecetaPorIdQueryHandler(IRecetaRepository repository)
        {
            _repository = repository;
        }

        public Task<RecetaDto> Handle(ObtenerRecetaPorIdQuery request, CancellationToken cancellationToken)
        {
            var receta = _repository.ObtenerPorId(request.Id);

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
