using System;
using System.Collections.Generic;
using System.Linq;

namespace Recetas.Domain.Entities
{
    /// <summary>
    /// Aggregate Root - Receta médica
    /// </summary>
    public class Receta
    {
        public int Id { get; private set; }
        
        /// <summary>
        /// ID de la cita que generó esta receta
        /// </summary>
        public int CitaId { get; private set; }
        
        /// <summary>
        /// ID del médico que emite la receta
        /// </summary>
        public int MedicoId { get; private set; }
        
        /// <summary>
        /// ID del paciente
        /// </summary>
        public int PacienteId { get; private set; }
        
        /// <summary>
        /// Diagnóstico médico
        /// </summary>
        public string Diagnostico { get; private set; }
        
        /// <summary>
        /// Fecha de emisión de la receta
        /// </summary>
        public DateTime FechaEmision { get; private set; }
        
        /// <summary>
        /// Observaciones adicionales
        /// </summary>
        public string Observaciones { get; private set; }
        
        /// <summary>
        /// Fecha hasta la cual la receta es válida
        /// </summary>
        public DateTime Vigencia { get; private set; }
        
        /// <summary>
        /// Lista de medicamentos en la receta
        /// </summary>
        private readonly List<DetalleReceta> _detalles = new List<DetalleReceta>();
        public IReadOnlyCollection<DetalleReceta> Detalles => _detalles.AsReadOnly();

        /// <summary>
        /// Constructor privado para EF
        /// </summary>
        private Receta() { }

        /// <summary>
        /// Constructor para crear una nueva receta
        /// </summary>
        public Receta(int citaId, int medicoId, int pacienteId, string diagnostico, 
                      DateTime vigencia, string observaciones = null)
        {
            ValidarCitaId(citaId);
            ValidarMedicoId(medicoId);
            ValidarPacienteId(pacienteId);
            ValidarDiagnostico(diagnostico);
            ValidarVigencia(vigencia);

            CitaId = citaId;
            MedicoId = medicoId;
            PacienteId = pacienteId;
            Diagnostico = diagnostico;
            FechaEmision = DateTime.Now;
            Vigencia = vigencia;
            Observaciones = observaciones;
        }

        /// <summary>
        /// Agrega un medicamento a la receta
        /// </summary>
        public void AgregarMedicamento(string nombreMedicamento, string dosis, 
                                       string frecuencia, string duracion, string indicaciones = null)
        {
            var detalle = new DetalleReceta(Id, nombreMedicamento, dosis, frecuencia, duracion, indicaciones);
            _detalles.Add(detalle);
        }

        /// <summary>
        /// Actualiza el diagnóstico y observaciones
        /// </summary>
        public void ActualizarDiagnostico(string diagnostico, string observaciones)
        {
            ValidarDiagnostico(diagnostico);
            Diagnostico = diagnostico;
            Observaciones = observaciones;
        }

        #region Validaciones

        private void ValidarCitaId(int citaId)
        {
            if (citaId <= 0)
                throw new ArgumentException("El ID de la cita debe ser mayor a 0");
        }

        private void ValidarMedicoId(int medicoId)
        {
            if (medicoId <= 0)
                throw new ArgumentException("El ID del médico debe ser mayor a 0");
        }

        private void ValidarPacienteId(int pacienteId)
        {
            if (pacienteId <= 0)
                throw new ArgumentException("El ID del paciente debe ser mayor a 0");
        }

        private void ValidarDiagnostico(string diagnostico)
        {
            if (string.IsNullOrWhiteSpace(diagnostico))
                throw new ArgumentException("El diagnóstico es obligatorio");

            if (diagnostico.Length > 1000)
                throw new ArgumentException("El diagnóstico no puede exceder 1000 caracteres");
        }

        private void ValidarVigencia(DateTime vigencia)
        {
            if (vigencia <= DateTime.Now)
                throw new ArgumentException("La vigencia de la receta debe ser futura");
        }

        #endregion
    }
}
