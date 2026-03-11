using Personas.Domain.Entities;
using Personas.Domain.ValueObjects;
using Personas.Infrastructure.Configurations;
using System.Data.Entity;

namespace Personas.Infrastructure.Persistence
{
    public class PersonasDbContext : DbContext
    {
        public PersonasDbContext() : base("PersonasDbConnection")
        {
            // Configuración de inicialización de base de datos
            Database.SetInitializer<PersonasDbContext>(null);

            // Configurar timeout de comandos (30 segundos)
            Database.CommandTimeout = 30;
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuración de esquema predeterminado
            modelBuilder.HasDefaultSchema("dbo");

            // Configuración de Value Objects
            modelBuilder.Configurations.Add(new DocumentoConfiguration());

            // Configuración de la entidad Persona
            modelBuilder.Entity<Persona>()
                .ToTable("Personas")
                .HasKey(p => p.Id);

            modelBuilder.Entity<Persona>()
                .Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Persona>()
                .Property(p => p.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Persona>()
                .Property(p => p.TipoPersona)
                .IsRequired();

            // Configuración de la entidad Usuario
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.NombreUsuario)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Activo)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.FechaCreacion)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .HasRequired(u => u.Persona)
                .WithMany()
                .HasForeignKey(u => u.PersonaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
