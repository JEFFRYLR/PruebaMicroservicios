using System;

namespace Citas.Application.DTOs
{
    /// <summary>
    /// DTO para transferir datos de Cita
    /// </summary>
    public class CitaDto
    {
        public int Id { get; set; }
        public DateTime FechaCita { get; set; }
        public string Lugar { get; set; }
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }
        public int Estado { get; set; }
        public string EstadoTexto { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
