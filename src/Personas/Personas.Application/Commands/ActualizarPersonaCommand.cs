using MediatR;
using Personas.Domain.Enums;

namespace Personas.Application.Commands
{
    /// <summary>
    /// Comando para actualizar una persona existente
    /// </summary>
    public class ActualizarPersonaCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
    }
}
