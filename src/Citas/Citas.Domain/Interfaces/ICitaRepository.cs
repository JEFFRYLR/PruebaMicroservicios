using Citas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Citas.Domain.Interfaces
{
    /// <summary>
    /// Contrato del repositorio de Citas (definido en el dominio)
    /// </summary>
    public interface ICitaRepository
    {
        /// <summary>
        /// Obtiene una cita por su ID
        /// </summary>
        Cita ObtenerPorId(int id);

        /// <summary>
        /// Obtiene todas las citas
        /// </summary>
        IQueryable<Cita> ObtenerTodas();

        /// <summary>
        /// Obtiene citas por médico
        /// </summary>
        IQueryable<Cita> ObtenerPorMedico(int medicoId);

        /// <summary>
        /// Obtiene citas por paciente
        /// </summary>
        IQueryable<Cita> ObtenerPorPaciente(int pacienteId);

        /// <summary>
        /// Obtiene citas por estado
        /// </summary>
        IQueryable<Cita> ObtenerPorEstado(Enums.EstadoCita estado);

        /// <summary>
        /// Crea una nueva cita
        /// </summary>
        void Crear(Cita cita);

        /// <summary>
        /// Actualiza una cita existente
        /// </summary>
        void Actualizar(Cita cita);

        /// <summary>
        /// Elimina una cita
        /// </summary>
        void Eliminar(int id);

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        void GuardarCambios();
    }
}
