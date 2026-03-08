using Citas.Domain.Enums;
using System;

namespace Citas.Domain.Entities
{
    /// <summary>
    /// Aggregate Root - Cita médica
    /// Representa una cita entre un médico y un paciente
    /// </summary>
    public class Cita
    {
        /// <summary>
        /// Identificador único de la cita
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Fecha y hora programada de la cita
        /// </summary>
        public DateTime FechaCita { get; private set; }

        /// <summary>
        /// Lugar físico donde se realizará la cita (ej: Consultorio 101)
        /// </summary>
        public string Lugar { get; private set; }

        /// <summary>
        /// ID del médico (referencia al microservicio Personas)
        /// </summary>
        public int MedicoId { get; private set; }

        /// <summary>
        /// ID del paciente (referencia al microservicio Personas)
        /// </summary>
        public int PacienteId { get; private set; }

        /// <summary>
        /// Estado actual de la cita
        /// </summary>
        public EstadoCita Estado { get; private set; }

        /// <summary>
        /// Motivo o descripción de la consulta
        /// </summary>
        public string Motivo { get; private set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; private set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; private set; }

        /// <summary>
        /// Constructor privado para EF
        /// </summary>
        private Cita() { }

        /// <summary>
        /// Constructor para agendar una nueva cita
        /// </summary>
        public Cita(DateTime fechaCita, string lugar, int medicoId, int pacienteId, string motivo)
        {
            ValidarFechaCita(fechaCita);
            ValidarLugar(lugar);
            ValidarMedicoId(medicoId);
            ValidarPacienteId(pacienteId);
            ValidarMotivo(motivo);

            FechaCita = fechaCita;
            Lugar = lugar;
            MedicoId = medicoId;
            PacienteId = pacienteId;
            Motivo = motivo;
            Estado = EstadoCita.Pendiente;
            FechaCreacion = DateTime.Now;
        }

        /// <summary>
        /// Actualiza el estado de la cita
        /// </summary>
        public void ActualizarEstado(EstadoCita nuevoEstado)
        {
            ValidarTransicionEstado(nuevoEstado);
            Estado = nuevoEstado;
            FechaActualizacion = DateTime.Now;
        }

        /// <summary>
        /// Finaliza la cita (cambia estado a Finalizada)
        /// </summary>
        public void Finalizar()
        {
            if (Estado != EstadoCita.EnProceso)
                throw new InvalidOperationException("Solo se pueden finalizar citas que estén en proceso");

            Estado = EstadoCita.Finalizada;
            FechaActualizacion = DateTime.Now;
        }

        /// <summary>
        /// Cancela la cita
        /// </summary>
        public void Cancelar()
        {
            if (Estado == EstadoCita.Finalizada)
                throw new InvalidOperationException("No se puede cancelar una cita finalizada");

            Estado = EstadoCita.Cancelada;
            FechaActualizacion = DateTime.Now;
        }

        /// <summary>
        /// Inicia la cita (cambia a En Proceso)
        /// </summary>
        public void IniciarAtencion()
        {
            if (Estado != EstadoCita.Pendiente)
                throw new InvalidOperationException("Solo se pueden iniciar citas pendientes");

            Estado = EstadoCita.EnProceso;
            FechaActualizacion = DateTime.Now;
        }

        #region Validaciones de Invariantes

        private void ValidarFechaCita(DateTime fechaCita)
        {
            if (fechaCita <= DateTime.Now)
                throw new ArgumentException("La fecha de la cita debe ser futura");
        }

        private void ValidarLugar(string lugar)
        {
            if (string.IsNullOrWhiteSpace(lugar))
                throw new ArgumentException("El lugar de la cita es obligatorio");

            if (lugar.Length > 200)
                throw new ArgumentException("El lugar no puede exceder 200 caracteres");
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

        private void ValidarMotivo(string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de la cita es obligatorio");

            if (motivo.Length > 500)
                throw new ArgumentException("El motivo no puede exceder 500 caracteres");
        }

        private void ValidarTransicionEstado(EstadoCita nuevoEstado)
        {
            // Reglas de transición de estados
            if (Estado == EstadoCita.Finalizada && nuevoEstado != EstadoCita.Finalizada)
                throw new InvalidOperationException("No se puede cambiar el estado de una cita finalizada");

            if (Estado == EstadoCita.Cancelada && nuevoEstado != EstadoCita.Cancelada)
                throw new InvalidOperationException("No se puede cambiar el estado de una cita cancelada");
        }

        #endregion
    }
}
