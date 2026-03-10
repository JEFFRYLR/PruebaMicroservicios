using Recetas.Domain.Entities;
using Recetas.Domain.Interfaces;
using Recetas.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Recetas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Recetas
    /// </summary>
    public class RecetaRepository : IRecetaRepository
    {
        private readonly RecetasDbContext _context;

        public RecetaRepository(RecetasDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Receta ObtenerPorId(int id)
        {
            return _context.Recetas
                .Include(r => r.Detalles)
                .FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Receta> ObtenerPorPaciente(int pacienteId)
        {
            return _context.Recetas
                .Include(r => r.Detalles)
                .Where(r => r.PacienteId == pacienteId)
                .ToList();
        }

        public IEnumerable<Receta> ObtenerPorMedico(int medicoId)
        {
            return _context.Recetas
                .Include(r => r.Detalles)
                .Where(r => r.MedicoId == medicoId)
                .ToList();
        }

        public Receta ObtenerPorCita(int citaId)
        {
            return _context.Recetas
                .Include(r => r.Detalles)
                .FirstOrDefault(r => r.CitaId == citaId);
        }

        public void Agregar(Receta receta)
        {
            _context.Recetas.Add(receta);
        }

        public void Crear(Receta receta)
        {
            _context.Recetas.Add(receta);
            _context.SaveChanges();
        }

        public void Actualizar(Receta receta)
        {
            _context.Entry(receta).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void GuardarCambios()
        {
            _context.SaveChanges();
        }
    }
}
