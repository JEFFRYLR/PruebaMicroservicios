using Personas.Domain.Entities;
using Personas.Domain.Interfaces;
using Personas.Infrastructure.Persistence;

using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Personas.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PersonasDbContext _context;

        public UsuarioRepository(PersonasDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                .Include(u => u.Persona)
                .Where(u => u.NombreUsuario == nombreUsuario)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Persona)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Persona> ObtenerPersonaPorIdAsync(int personaId)
        {
            return await _context.Personas
                .Where(p => p.Id == personaId)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
