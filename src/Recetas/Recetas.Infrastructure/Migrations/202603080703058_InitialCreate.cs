namespace Recetas.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetallesReceta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecetaId = c.Int(nullable: false),
                        NombreMedicamento = c.String(nullable: false, maxLength: 200),
                        Dosis = c.String(nullable: false, maxLength: 100),
                        Frecuencia = c.String(nullable: false, maxLength: 100),
                        Duracion = c.String(nullable: false, maxLength: 100),
                        Indicaciones = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Recetas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CitaId = c.Int(nullable: false),
                        MedicoId = c.Int(nullable: false),
                        PacienteId = c.Int(nullable: false),
                        Diagnostico = c.String(nullable: false, maxLength: 1000),
                        FechaEmision = c.DateTime(nullable: false),
                        Observaciones = c.String(maxLength: 500),
                        Vigencia = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Recetas");
            DropTable("dbo.DetallesReceta");
        }
    }
}
