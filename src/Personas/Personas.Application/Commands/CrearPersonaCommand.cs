using MediatR;
using Personas.Domain.Enums;

namespace Personas.Application.Commands
{
    /// <summary>
    /// Comando para crear una nueva persona (médico o paciente)
    /// </summary>
    public class CrearPersonaCommand : IRequest<int>
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public TipoPersona TipoPersona { get; set; }
    }
}
