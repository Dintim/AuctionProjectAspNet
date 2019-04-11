namespace EAuction.Infrastructure.Migrations.EAuctionDbConfiguration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinRatingColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "MinRatingForParticipant", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "MinRatingForParticipant");
        }
    }
}
