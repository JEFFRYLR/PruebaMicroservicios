using Personas.Domain.Entities;
using System.Collections.Generic;

namespace Personas.Domain.Interfaces
{
    public interface IPersonaRepository
    {
        Persona ObtenerPorId(int id);

        IEnumerable<Persona> ObtenerTodos();

        void Crear(Persona persona);

        void Actualizar(Persona persona);

        void Eliminar(int id);
    }
}
