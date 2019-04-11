namespace EAuction.Infrastructure.Migrations.EAuctionDbConfiguration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuctionFiles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(),
                        Extension = c.String(),
                        Content = c.Binary(),
                        CreatedAt = c.DateTime(nullable: false),
                        AuctionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auctions", t => t.AuctionId, cascadeDelete: true)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(),
                        ShippingAddress = c.String(),
                        ShippingConditions = c.String(),
                        StartPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceStep = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                        FinishDateAtActual = c.DateTime(),
                        Status = c.Int(nullable: false),
                        AuctionTypeId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuctionTypes", t => t.AuctionTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.AuctionTypeId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.AuctionTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        AuctionId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Auctions", t => t.AuctionId, cascadeDelete: true)
                .Index(t => t.AuctionId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FullName = c.String(),
                        IdentificationNumber = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        Contacts = c.String(),
                        Site = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        OrganizationTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganizationTypes", t => t.OrganizationTypeId, cascadeDelete: true)
                .Index(t => t.OrganizationTypeId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        DoB = c.DateTime(nullable: false),
                        Email = c.String(),
                        EmployeePositionId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeePositions", t => t.EmployeePositionId, cascadeDelete: true)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.EmployeePositionId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.EmployeePositions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrganizationFiles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(),
                        Extension = c.String(),
                        Content = c.Binary(),
                        CreatedAt = c.DateTime(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.OrganizationRatings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Score = c.Double(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.OrganizationTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TransactionType = c.Int(nullable: false),
                        Sum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "AuctionId", "dbo.Auctions");
            DropForeignKey("dbo.Transactions", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "OrganizationTypeId", "dbo.OrganizationTypes");
            DropForeignKey("dbo.OrganizationRatings", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationFiles", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Employees", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Employees", "EmployeePositionId", "dbo.EmployeePositions");
            DropForeignKey("dbo.Bids", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Auctions", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Auctions", "AuctionTypeId", "dbo.AuctionTypes");
            DropForeignKey("dbo.AuctionFiles", "AuctionId", "dbo.Auctions");
            DropIndex("dbo.Transactions", new[] { "OrganizationId" });
            DropIndex("dbo.OrganizationRatings", new[] { "OrganizationId" });
            DropIndex("dbo.OrganizationFiles", new[] { "OrganizationId" });
            DropIndex("dbo.Employees", new[] { "OrganizationId" });
            DropIndex("dbo.Employees", new[] { "EmployeePositionId" });
            DropIndex("dbo.Organizations", new[] { "OrganizationTypeId" });
            DropIndex("dbo.Bids", new[] { "OrganizationId" });
            DropIndex("dbo.Bids", new[] { "AuctionId" });
            DropIndex("dbo.Auctions", new[] { "OrganizationId" });
            DropIndex("dbo.Auctions", new[] { "AuctionTypeId" });
            DropIndex("dbo.AuctionFiles", new[] { "AuctionId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.OrganizationTypes");
            DropTable("dbo.OrganizationRatings");
            DropTable("dbo.OrganizationFiles");
            DropTable("dbo.EmployeePositions");
            DropTable("dbo.Employees");
            DropTable("dbo.Organizations");
            DropTable("dbo.Bids");
            DropTable("dbo.AuctionTypes");
            DropTable("dbo.Auctions");
            DropTable("dbo.AuctionFiles");
        }
    }
}
