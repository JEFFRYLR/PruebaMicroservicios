using MediatR;
using Recetas.Application.Commands;
using Recetas.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Recetas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para agregar medicamento a receta existente
    /// </summary>
    public class AgregarMedicamentoCommandHandler : IRequestHandler<AgregarMedicamentoCommand, Unit>
    {
        private readonly IRecetaRepository _repository;

        public AgregarMedicamentoCommandHandler(IRecetaRepository repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(AgregarMedicamentoCommand request, CancellationToken cancellationToken)
        {
            var receta = _repository.ObtenerPorId(request.RecetaId);

            if (receta == null)
                throw new Exception($"Receta con ID {request.RecetaId} no encontrada");

            receta.AgregarMedicamento(
                request.NombreMedicamento,
                request.Dosis,
                request.Frecuencia,
                request.Duracion,
                request.Indicaciones
            );

            _repository.Actualizar(receta);

            return Task.FromResult(Unit.Value);
        }
    }
}
