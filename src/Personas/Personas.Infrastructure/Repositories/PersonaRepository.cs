using Personas.Domain.Entities;
using Personas.Domain.Interfaces;
using Personas.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Personas.Infrastructure.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly PersonasDbContext _context;

        public PersonaRepository(PersonasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Persona> ObtenerTodos()
        {
            return _context.Personas.ToList();
        }

        public Persona ObtenerPorId(int id)
        {
            return _context.Personas.Find(id);
        }

        public void Crear(Persona persona)
        {
            _context.Personas.Add(persona);
            _context.SaveChanges();
        }

        public void Actualizar(Persona persona)
        {
            _context.Entry(persona).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var persona = _context.Personas.Find(id);

            if (persona != null)
            {
                _context.Personas.Remove(persona);
                _context.SaveChanges();
            }
        }
    }
}
