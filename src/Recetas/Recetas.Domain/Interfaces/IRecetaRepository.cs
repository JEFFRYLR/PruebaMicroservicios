using Recetas.Domain.Entities;
using System.Collections.Generic;

namespace Recetas.Domain.Interfaces
{
    /// <summary>
    /// Contrato del repositorio de Recetas
    /// </summary>
    public interface IRecetaRepository
    {
        Receta ObtenerPorId(int id);
        IEnumerable<Receta> ObtenerPorPaciente(int pacienteId);
        IEnumerable<Receta> ObtenerPorMedico(int medicoId);
        Receta ObtenerPorCita(int citaId);
        void Crear(Receta receta);
        void Actualizar(Receta receta);
    }
}
