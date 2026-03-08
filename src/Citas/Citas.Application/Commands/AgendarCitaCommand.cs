using MediatR;
using System;

namespace Citas.Application.Commands
{
    /// <summary>
    /// Command para agendar una nueva cita
    /// </summary>
    public class AgendarCitaCommand : IRequest<int>
    {
        public DateTime FechaCita { get; set; }
        public string Lugar { get; set; }
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }
        public string Motivo { get; set; }
    }
}
