namespace Recetas.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Recetas.Infrastructure.Persistence.RecetasDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Recetas.Infrastructure.Persistence.RecetasDbContext";
        }

        protected override void Seed(Recetas.Infrastructure.Persistence.RecetasDbContext context)
        {
            // Este método se ejecutará después de migrar a la última versión.
            // Puede usar el método DbSet<T>.AddOrUpdate() para evitar crear datos de inicialización duplicados.
        }
    }
}
