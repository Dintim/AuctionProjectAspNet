namespace EAuction.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserPasswordHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SetupDate = c.DateTime(nullable: false),
                        InvalidatedDate = c.DateTime(nullable: false),
                        Password = c.String(),
                        ApplicationUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        FailedSignInCount = c.Int(nullable: false),
                        AssosiatedEmployeeId = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserSignInHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SignInTime = c.DateTime(nullable: false),
                        MachineIp = c.String(),
                        IpToGeoCountry = c.String(),
                        IpToGeoCity = c.String(),
                        IpToGeoLatitude = c.Double(nullable: false),
                        IpToGeoLongitude = c.String(),
                        ApplicationUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserSignInHistories", "ApplicationUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserPasswordHistories", "ApplicationUserId", "dbo.ApplicationUsers");
            DropIndex("dbo.ApplicationUserSignInHistories", new[] { "ApplicationUserId" });
            DropIndex("dbo.ApplicationUserPasswordHistories", new[] { "ApplicationUserId" });
            DropTable("dbo.ApplicationUserSignInHistories");
            DropTable("dbo.ApplicationUsers");
            DropTable("dbo.ApplicationUserPasswordHistories");
        }
    }
}
