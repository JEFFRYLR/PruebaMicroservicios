namespace Citas.Domain.Enums
{
    /// <summary>
    /// Estados posibles de una cita médica
    /// </summary>
    public enum EstadoCita
    {
        /// <summary>
        /// Cita agendada pero no ha iniciado
        /// </summary>
        Pendiente = 1,

        /// <summary>
        /// Cita en curso, médico atendiendo al paciente
        /// </summary>
        EnProceso = 2,

        /// <summary>
        /// Cita completada, requiere ingreso de receta
        /// </summary>
        Finalizada = 3,

        /// <summary>
        /// Cita cancelada por médico o paciente
        /// </summary>
        Cancelada = 4
    }
}
