using Personas.Domain.ValueObjects;
using System.Data.Entity.ModelConfiguration;

namespace Personas.Infrastructure.Configurations
{
    public class DocumentoConfiguration : ComplexTypeConfiguration<Documento>
    {
        public DocumentoConfiguration()
        {
            Property(d => d.Numero)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}