using MediatR;
using Recetas.Application.DTOs;
using Recetas.Application.Queries;
using Recetas.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Recetas.Application.QueryHandlers
{
    /// <summary>
    /// Handler para obtener recetas por paciente
    /// </summary>
    public class ObtenerRecetasPorPacienteQueryHandler : IRequestHandler<ObtenerRecetasPorPacienteQuery, IEnumerable<RecetaDto>>
    {
        private readonly IRecetaRepository _repository;

        public ObtenerRecetasPorPacienteQueryHandler(IRecetaRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<RecetaDto>> Handle(ObtenerRecetasPorPacienteQuery request, CancellationToken cancellationToken)
        {
            var recetas = _repository.ObtenerPorPaciente(request.PacienteId);

            var resultado = recetas.Select(r => new RecetaDto
            {
                Id = r.Id,
                CitaId = r.CitaId,
                MedicoId = r.MedicoId,
                PacienteId = r.PacienteId,
                Diagnostico = r.Diagnostico,
                FechaEmision = r.FechaEmision,
                Observaciones = r.Observaciones,
                Vigencia = r.Vigencia,
                Detalles = r.Detalles.Select(d => new DetalleRecetaDto
                {
                    Id = d.Id,
                    RecetaId = d.RecetaId,
                    NombreMedicamento = d.NombreMedicamento,
                    Dosis = d.Dosis,
                    Frecuencia = d.Frecuencia,
                    Duracion = d.Duracion,
                    Indicaciones = d.Indicaciones
                }).ToList()
            });

            return Task.FromResult(resultado);
        }
    }
}
