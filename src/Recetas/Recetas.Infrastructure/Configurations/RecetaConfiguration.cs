using Recetas.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Recetas.Infrastructure.Configurations
{
    /// <summary>
    /// Configuración Fluent API para Receta
    /// </summary>
    public class RecetaConfiguration : EntityTypeConfiguration<Receta>
    {
        public RecetaConfiguration()
        {
            ToTable("Recetas");

            HasKey(r => r.Id);

            Property(r => r.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(r => r.CitaId)
                .IsRequired();

            Property(r => r.MedicoId)
                .IsRequired();

            Property(r => r.PacienteId)
                .IsRequired();

            Property(r => r.Diagnostico)
                .IsRequired()
                .HasMaxLength(1000);

            Property(r => r.FechaEmision)
                .IsRequired();

            Property(r => r.Observaciones)
                .HasMaxLength(500);

            Property(r => r.Vigencia)
                .IsRequired();

            // No se necesita configurar explícitamente la navegación de la colección
            // Entity Framework la detectará automáticamente por convención
        }
    }
}
