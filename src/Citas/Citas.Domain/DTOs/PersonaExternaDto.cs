namespace Citas.Domain.DTOs
{
    /// <summary>
    /// DTO para representar datos básicos de una persona externa del microservicio de Personas
    /// </summary>
    public class PersonaExternaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int TipoPersona { get; set; }
    }
}