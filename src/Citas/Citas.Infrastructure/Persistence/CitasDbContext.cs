using Citas.Domain.Entities;
using System.Data.Entity;

namespace Citas.Infrastructure.Persistence
{
    /// <summary>
    /// Contexto de Entity Framework para Citas
    /// </summary>
    public class CitasDbContext : DbContext
    {
        public CitasDbContext() : base("name=CitasConnection")
        {
        }

        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Configurations.CitaConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
