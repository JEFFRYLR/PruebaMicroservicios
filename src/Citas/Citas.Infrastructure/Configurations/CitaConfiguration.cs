using Citas.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Citas.Infrastructure.Configurations
{
    /// <summary>
    /// Configuración de la entidad Cita para Entity Framework
    /// </summary>
    public class CitaConfiguration : EntityTypeConfiguration<Cita>
    {
        public CitaConfiguration()
        {
            ToTable("Citas");

            HasKey(c => c.Id);

            Property(c => c.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(c => c.FechaCita)
                .IsRequired();

            Property(c => c.Lugar)
                .IsRequired()
                .HasMaxLength(200);

            Property(c => c.MedicoId)
                .IsRequired();

            Property(c => c.PacienteId)
                .IsRequired();

            Property(c => c.Estado)
                .IsRequired();

            Property(c => c.Motivo)
                .IsRequired()
                .HasMaxLength(500);

            Property(c => c.FechaCreacion)
                .IsRequired();

            Property(c => c.FechaActualizacion)
                .IsOptional();
        }
    }
}
