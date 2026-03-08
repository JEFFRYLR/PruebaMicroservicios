namespace Recetas.Application.DTOs
{
    /// <summary>
    /// DTO para detalle de medicamento
    /// </summary>
    public class DetalleRecetaDto
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public string NombreMedicamento { get; set; }
        public string Dosis { get; set; }
        public string Frecuencia { get; set; }
        public string Duracion { get; set; }
        public string Indicaciones { get; set; }
    }
}
