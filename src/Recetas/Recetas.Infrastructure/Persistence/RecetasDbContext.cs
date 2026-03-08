using Recetas.Domain.Entities;
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
            modelBuilder.Configurations.Add(new Configurations.RecetaConfiguration());
            modelBuilder.Configurations.Add(new Configurations.DetalleRecetaConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
