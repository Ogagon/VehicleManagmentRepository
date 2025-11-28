namespace MonoTask.Service.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VehicleMakes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 100),
                    Abrv = c.String(nullable: false, maxLength: 20),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.VehicleModels",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    MakeId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 100),
                    Abrv = c.String(nullable: false, maxLength: 20),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleMakes", t => t.MakeId, cascadeDelete: true)
                .Index(t => t.MakeId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.VehicleModels", "MakeId", "dbo.VehicleMakes");
            DropIndex("dbo.VehicleModels", new[] { "MakeId" });
            DropTable("dbo.VehicleModels");
            DropTable("dbo.VehicleMakes");
        }
    }
}
