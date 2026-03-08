using Personas.Application.DTOs;
using System.Collections.Generic;

namespace Personas.Application.Interfaces
{
    public interface IPersonaService
    {
        IEnumerable<PersonaDto> ObtenerTodas();
        PersonaDto ObtenerPorId(int id);
        IEnumerable<PersonaDto> ObtenerMedicos();
        IEnumerable<PersonaDto> ObtenerPacientes();
        void Crear(PersonaDto dto);
        void Actualizar(int id, PersonaDto dto);
        void Eliminar(int id);
    }
}
