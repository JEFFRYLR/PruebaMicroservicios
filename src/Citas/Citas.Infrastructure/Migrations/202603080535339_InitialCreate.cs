namespace Citas.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Citas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaCita = c.DateTime(nullable: false),
                        Lugar = c.String(nullable: false, maxLength: 200),
                        MedicoId = c.Int(nullable: false),
                        PacienteId = c.Int(nullable: false),
                        Estado = c.Int(nullable: false),
                        Motivo = c.String(nullable: false, maxLength: 500),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaActualizacion = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Citas");
        }
    }
}
