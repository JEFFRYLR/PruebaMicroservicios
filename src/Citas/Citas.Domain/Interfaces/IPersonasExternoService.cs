using Citas.Domain.DTOs;
using System.Threading.Tasks;

namespace Citas.Domain.Interfaces
{
    /// <summary>
    /// Contrato para comunicación con el microservicio de Personas
    /// Define las operaciones necesarias para validar médicos y pacientes
    /// </summary>
    public interface IPersonasExternoService
    {
        /// <summary>
        /// Valida si existe un médico con el ID especificado
        /// </summary>
        /// <param name="medicoId">ID del médico a validar</param>
        /// <returns>True si existe y está activo, false en caso contrario</returns>
        Task<bool> ExisteMedicoAsync(int medicoId);

        /// <summary>
        /// Valida si existe un paciente con el ID especificado
        /// </summary>
        /// <param name="pacienteId">ID del paciente a validar</param>
        /// <returns>True si existe y está activo, false en caso contrario</returns>
        Task<bool> ExistePacienteAsync(int pacienteId);

        /// <summary>
        /// Obtiene los datos completos de una persona por ID
        /// </summary>
        /// <param name="personaId">ID de la persona</param>
        /// <returns>DTO con datos de la persona o null si no existe</returns>
        Task<PersonaExternaDto> ObtenerPersonaPorIdAsync(int personaId);

        /// <summary>
        /// Configura el token de autorización para las peticiones
        /// </summary>
        /// <param name="token">Token de autorización</param>
        void SetAuthorizationToken(string token);
    }
}