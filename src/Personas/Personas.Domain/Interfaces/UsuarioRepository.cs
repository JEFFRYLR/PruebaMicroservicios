using Personas.Domain.Entities;
using System.Threading.Tasks;

namespace Personas.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task<Usuario> ObtenerPorIdAsync(int id);
        Task<Persona> ObtenerPersonaPorIdAsync(int personaId);
        Task<Usuario> CrearAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }
}
