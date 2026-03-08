using Personas.Domain.Enums;

namespace Personas.Application.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de personas (médicos y pacientes)
    /// </summary>
    public class PersonaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public TipoPersona TipoPersona { get; set; }
    }
}
