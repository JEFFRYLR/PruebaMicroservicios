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
                .FirstOrDefault(r => r.CitaId == citaId);
        }

        public void Agregar(Receta receta)
        {
            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Agregando receta al contexto...");
            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] CitaId: {0}", receta.CitaId);
            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Estado ANTES de Add: {0}", 
                _context.Entry(receta).State);
            
            _context.Recetas.Add(receta);
            
            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Estado DESPUÉS de Add: {0}", 
                _context.Entry(receta).State);
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
            try
            {
                System.Diagnostics.Debug.WriteLine("==========================================");
                System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Iniciando SaveChanges...");
                System.Diagnostics.Trace.WriteLine("[REPOSITORIO] Iniciando SaveChanges...");
                
                // VERIFICAR ENTIDADES PENDIENTES
                var pendientes = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || 
                                e.State == EntityState.Modified || 
                                e.State == EntityState.Deleted)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Entidades pendientes: {0}", pendientes.Count);
                System.Diagnostics.Trace.WriteLine(string.Format("[REPOSITORIO] Entidades pendientes: {0}", pendientes.Count));
                
                foreach (var entry in pendientes)
                {
                    var entityName = entry.Entity.GetType().Name;
                    System.Diagnostics.Debug.WriteLine("[REPOSITORIO]   - {0}: {1}", entityName, entry.State);
                    System.Diagnostics.Trace.WriteLine(string.Format("[REPOSITORIO]   - {0}: {1}", entityName, entry.State));
                }
                
                // CRITICO: Usar transaccion explicita para garantizar commit
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Transaccion iniciada");
                        System.Diagnostics.Trace.WriteLine("[REPOSITORIO] Transaccion iniciada");
                        
                        var cambios = _context.SaveChanges();
                        
                        System.Diagnostics.Debug.WriteLine("[REPOSITORIO] SaveChanges ejecutado. Registros afectados: {0}", cambios);
                        System.Diagnostics.Trace.WriteLine(string.Format("[REPOSITORIO] SaveChanges ejecutado. Registros afectados: {0}", cambios));
                        
                        if (cambios == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] ADVERTENCIA: SaveChanges retorno 0 cambios");
                            System.Diagnostics.Trace.WriteLine("[REPOSITORIO] ADVERTENCIA: SaveChanges retorno 0 cambios");
                        }
                        
                        // VERIFICAR ESTADOS DESPUÉS DE SAVECHANGES
                        foreach (var entry in pendientes)
                        {
                            var entityName = entry.Entity.GetType().Name;
                            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Estado POST-SaveChanges - {0}: {1}", 
                                entityName, entry.State);
                        }
                        
                        // COMMIT EXPLICITO
                        transaction.Commit();
                        
                        System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Transaction.Commit() ejecutado EXITOSAMENTE");
                        System.Diagnostics.Trace.WriteLine("[REPOSITORIO] Transaction.Commit() ejecutado EXITOSAMENTE");
                        
                        // VERIFICACIÓN CRÍTICA: Intentar leer inmediatamente
                        if (pendientes.Any(e => e.Entity is Receta))
                        {
                            var recetaAgregada = pendientes.First(e => e.Entity is Receta).Entity as Receta;
                            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] ID generado para Receta: {0}", recetaAgregada.Id);
                            
                            // Intentar buscar la receta recién insertada
                            var verificacion = _context.Recetas.Find(recetaAgregada.Id);
                            System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Verificación inmediata en contexto: {0}", 
                                verificacion != null ? "ENCONTRADA" : "NO ENCONTRADA");
                            
                            System.Diagnostics.Trace.WriteLine(string.Format("[REPOSITORIO] ID generado: {0}, Verificación: {1}", 
                                recetaAgregada.Id, 
                                verificacion != null ? "ENCONTRADA" : "NO ENCONTRADA"));
                        }
                    }
                    catch (Exception txEx)
                    {
                        System.Diagnostics.Debug.WriteLine("[REPOSITORIO] ERROR en transaccion: {0}", txEx.Message);
                        System.Diagnostics.Trace.WriteLine(string.Format("[REPOSITORIO] ERROR en transaccion: {0}", txEx.Message));
                        
                        transaction.Rollback();
                        
                        System.Diagnostics.Debug.WriteLine("[REPOSITORIO] Rollback ejecutado");
                        System.Diagnostics.Trace.WriteLine("[REPOSITORIO] Rollback ejecutado");
                        
                        throw;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine("[REPOSITORIO] GuardarCambios completado exitosamente");
                System.Diagnostics.Debug.WriteLine("==========================================");
                System.Diagnostics.Trace.WriteLine("[REPOSITORIO] GuardarCambios completado exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR REPOSITORIO] SaveChanges fallo: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("[ERROR REPOSITORIO] StackTrace: " + ex.StackTrace);
                
                System.Diagnostics.Trace.WriteLine("[ERROR REPOSITORIO] SaveChanges fallo: " + ex.Message);
                System.Diagnostics.Trace.WriteLine("[ERROR REPOSITORIO] StackTrace: " + ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR REPOSITORIO] Inner: " + ex.InnerException.Message);
                    System.Diagnostics.Trace.WriteLine("[ERROR REPOSITORIO] Inner: " + ex.InnerException.Message);
                    
                    if (ex.InnerException.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine("[ERROR REPOSITORIO] Inner2: " + ex.InnerException.InnerException.Message);
                        System.Diagnostics.Trace.WriteLine("[ERROR REPOSITORIO] Inner2: " + ex.InnerException.InnerException.Message);
                    }
                }
                throw;
            }
        }
    }
}
