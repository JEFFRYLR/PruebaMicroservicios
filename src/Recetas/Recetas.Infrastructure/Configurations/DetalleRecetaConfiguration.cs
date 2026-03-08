using Recetas.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Recetas.Infrastructure.Configurations
{
    /// <summary>
    /// Configuración Fluent API para DetalleReceta
    /// </summary>
    public class DetalleRecetaConfiguration : EntityTypeConfiguration<DetalleReceta>
    {
        public DetalleRecetaConfiguration()
        {
            ToTable("DetallesReceta");

            HasKey(d => d.Id);

            Property(d => d.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(d => d.RecetaId)
                .IsRequired();

            Property(d => d.NombreMedicamento)
                .IsRequired()
                .HasMaxLength(200);

            Property(d => d.Dosis)
                .IsRequired()
                .HasMaxLength(100);

            Property(d => d.Frecuencia)
                .IsRequired()
                .HasMaxLength(100);

            Property(d => d.Duracion)
                .IsRequired()
                .HasMaxLength(100);

            Property(d => d.Indicaciones)
                .HasMaxLength(500);
        }
    }
}
