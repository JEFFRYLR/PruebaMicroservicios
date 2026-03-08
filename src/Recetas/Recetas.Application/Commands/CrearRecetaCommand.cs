using MediatR;
using System;
using System.Collections.Generic;

namespace Recetas.Application.Commands
{
    /// <summary>
    /// Comando para crear una nueva receta con medicamentos
    /// </summary>
    public class CrearRecetaCommand : IRequest<int>
    {
        public int CitaId { get; set; }
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }
        public string Diagnostico { get; set; }
        public DateTime Vigencia { get; set; }
        public string Observaciones { get; set; }
        public List<MedicamentoDto> Medicamentos { get; set; }
    }

    public class MedicamentoDto
    {
        public string NombreMedicamento { get; set; }
        public string Dosis { get; set; }
        public string Frecuencia { get; set; }
        public string Duracion { get; set; }
        public string Indicaciones { get; set; }
    }
}
