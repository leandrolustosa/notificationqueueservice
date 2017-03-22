namespace AInBox.Messaging.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationservice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "email",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        ReferenceName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ReferenceId = c.Int(nullable: false),
                        ReferenceParameters = c.String(unicode: false, storeType: "text"),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        ScheduleDate = c.DateTime(precision: 0),
                        NotificationDate = c.DateTime(precision: 0),
                        Priority = c.Int(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        SubjectEncoding = c.String(maxLength: 10, storeType: "nvarchar"),
                        BodyEncoding = c.String(maxLength: 10, storeType: "nvarchar"),
                        IsBodyHtml = c.Boolean(nullable: false),
                        To = c.String(nullable: false, unicode: false, storeType: "text"),
                        Cc = c.String(unicode: false, storeType: "text"),
                        Bco = c.String(unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("module", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "module",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EnterpriseId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        FromName = c.String(maxLength: 40, storeType: "nvarchar"),
                        FromEmail = c.String(maxLength: 40, storeType: "nvarchar"),
                        RestrictIps = c.String(maxLength: 40, storeType: "nvarchar"),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("enterprise", t => t.EnterpriseId, cascadeDelete: true)
                .Index(t => new { t.EnterpriseId, t.Name }, unique: true, name: "idx_enterpriseId_name");
            
            CreateTable(
                "enterprise",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        Username = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        Password = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        FromName = c.String(maxLength: 40, storeType: "nvarchar"),
                        FromEmail = c.String(maxLength: 40, storeType: "nvarchar"),
                        Host = c.String(maxLength: 40, storeType: "nvarchar"),
                        Port = c.Int(),
                        ServerUsername = c.String(maxLength: 20, storeType: "nvarchar"),
                        ServerPassword = c.String(maxLength: 60, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name)
                .Index(t => t.Username, unique: true, name: "idx_username");
            
        }
        
        public override void Down()
        {
            DropForeignKey("email", "ModuleId", "module");
            DropForeignKey("module", "EnterpriseId", "enterprise");
            DropIndex("enterprise", "idx_username");
            DropIndex("enterprise", new[] { "Name" });
            DropIndex("module", "idx_enterpriseId_name");
            DropIndex("email", new[] { "ModuleId" });
            DropTable("enterprise");
            DropTable("module");
            DropTable("email");
        }
    }
}
