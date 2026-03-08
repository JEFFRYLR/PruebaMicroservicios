using Citas.Domain.Entities;
using Citas.Domain.Enums;
using Citas.Domain.Interfaces;
using Citas.Infrastructure.Persistence;
using System;
using System.Linq;

namespace Citas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Citas con Entity Framework
    /// </summary>
    public class CitaRepository : ICitaRepository
    {
        private readonly CitasDbContext _context;

        public CitaRepository(CitasDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Cita ObtenerPorId(int id)
        {
            return _context.Citas.Find(id);
        }

        public IQueryable<Cita> ObtenerTodas()
        {
            return _context.Citas.AsQueryable();
        }

        public IQueryable<Cita> ObtenerPorMedico(int medicoId)
        {
            return _context.Citas.Where(c => c.MedicoId == medicoId);
        }

        public IQueryable<Cita> ObtenerPorPaciente(int pacienteId)
        {
            return _context.Citas.Where(c => c.PacienteId == pacienteId);
        }

        public IQueryable<Cita> ObtenerPorEstado(EstadoCita estado)
        {
            return _context.Citas.Where(c => c.Estado == estado);
        }

        public void Crear(Cita cita)
        {
            _context.Citas.Add(cita);
        }

        public void Actualizar(Cita cita)
        {
            _context.Entry(cita).State = System.Data.Entity.EntityState.Modified;
        }

        public void Eliminar(int id)
        {
            var cita = ObtenerPorId(id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
            }
        }

        public void GuardarCambios()
        {
            _context.SaveChanges();
        }
    }
}
