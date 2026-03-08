using Personas.Domain.Interfaces;
using Personas.Infrastructure.Persistence;
using Personas.Infrastructure.Repositories;
using System;

namespace Personas.Infrastructure.Factories
{
    public interface IRepositoryFactory : IDisposable
    {
        IPersonaRepository CreatePersonaRepository();
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly PersonasDbContext _context;

        public RepositoryFactory()
        {
            _context = new PersonasDbContext();
        }

        public IPersonaRepository CreatePersonaRepository()
        {
            return new PersonaRepository(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
