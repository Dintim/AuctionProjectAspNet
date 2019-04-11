namespace EAuction.Infrastructure.Migrations.EAuctionDbConfiguration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBidStatusTab : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BidStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Bids", "BidStatusId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Bids", "BidStatusId");
            AddForeignKey("dbo.Bids", "BidStatusId", "dbo.BidStatus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "BidStatusId", "dbo.BidStatus");
            DropIndex("dbo.Bids", new[] { "BidStatusId" });
            DropColumn("dbo.Bids", "BidStatusId");
            DropTable("dbo.BidStatus");
        }
    }
}
