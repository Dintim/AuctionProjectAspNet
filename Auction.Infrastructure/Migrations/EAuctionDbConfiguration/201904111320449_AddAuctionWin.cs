namespace EAuction.Infrastructure.Migrations.EAuctionDbConfiguration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctionWin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuctionWins",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SetupDate = c.DateTime(nullable: false),
                        Note = c.String(),
                        AuctionId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Auctions", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuctionWins", "Id", "dbo.Auctions");
            DropForeignKey("dbo.AuctionWins", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.AuctionWins", new[] { "OrganizationId" });
            DropIndex("dbo.AuctionWins", new[] { "Id" });
            DropTable("dbo.AuctionWins");
        }
    }
}
