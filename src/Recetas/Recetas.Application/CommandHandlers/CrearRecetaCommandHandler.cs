using MediatR;
using Recetas.Application.Commands;
using Recetas.Domain.Entities;
using Recetas.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Recetas.Application.CommandHandlers
{
    /// <summary>
    /// Handler para crear una receta con medicamentos
    /// </summary>
    public class CrearRecetaCommandHandler : IRequestHandler<CrearRecetaCommand, int>
    {
        private readonly IRecetaRepository _repository;

        public CrearRecetaCommandHandler(IRecetaRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Handle(CrearRecetaCommand request, CancellationToken cancellationToken)
        {
            // Crear la receta
            var receta = new Receta(
                request.CitaId,
                request.MedicoId,
                request.PacienteId,
                request.Diagnostico,
                request.Vigencia,
                request.Observaciones
            );

            // Agregar medicamentos si existen
            if (request.Medicamentos != null)
            {
                foreach (var medicamento in request.Medicamentos)
                {
                    receta.AgregarMedicamento(
                        medicamento.NombreMedicamento,
                        medicamento.Dosis,
                        medicamento.Frecuencia,
                        medicamento.Duracion,
                        medicamento.Indicaciones
                    );
                }
            }

            _repository.Crear(receta);

            return Task.FromResult(receta.Id);
        }
    }
}
