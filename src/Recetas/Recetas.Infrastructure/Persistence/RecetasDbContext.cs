using Recetas.Domain.Entities;
using Recetas.Infrastructure.Configurations;
using System.Data.Entity;

namespace Recetas.Infrastructure.Persistence
{
    /// <summary>
    /// DbContext de Entity Framework para Recetas
    /// </summary>
    public class RecetasDbContext : DbContext
    {
        public RecetasDbContext() : base("name=RecetasConnection")
        {
        }

        public DbSet<Receta> Recetas { get; set; }
        public DbSet<DetalleReceta> DetallesReceta { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new RecetaConfiguration());
            modelBuilder.Configurations.Add(new DetalleRecetaConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
