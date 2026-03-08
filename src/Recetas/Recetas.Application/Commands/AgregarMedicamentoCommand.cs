using MediatR;

namespace Recetas.Application.Commands
{
    /// <summary>
    /// Comando para agregar medicamento a receta existente
    /// </summary>
    public class AgregarMedicamentoCommand : IRequest<Unit>
    {
        public int RecetaId { get; set; }
        public string NombreMedicamento { get; set; }
        public string Dosis { get; set; }
        public string Frecuencia { get; set; }
        public string Duracion { get; set; }
        public string Indicaciones { get; set; }
    }
}
