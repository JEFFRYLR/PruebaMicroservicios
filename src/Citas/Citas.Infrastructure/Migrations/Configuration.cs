using System.Data.Entity.Migrations;

namespace Citas.Infrastructure.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Citas.Infrastructure.Persistence.CitasDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Citas.Infrastructure.Persistence.CitasDbContext";
        }

        protected override void Seed(Citas.Infrastructure.Persistence.CitasDbContext context)
        {
            // Seed data si es necesario
        }
    }
}
