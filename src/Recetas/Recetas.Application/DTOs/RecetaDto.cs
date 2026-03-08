using System;
using System.Collections.Generic;

namespace Recetas.Application.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Receta
    /// </summary>
    public class RecetaDto
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }
        public string Diagnostico { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Observaciones { get; set; }
        public DateTime Vigencia { get; set; }
        public List<DetalleRecetaDto> Detalles { get; set; }
    }
}
