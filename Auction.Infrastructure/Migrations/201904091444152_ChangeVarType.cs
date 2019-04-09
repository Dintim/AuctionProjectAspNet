namespace EAuction.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVarType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApplicationUserSignInHistories", "IpToGeoLongitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationUserSignInHistories", "IpToGeoLongitude", c => c.String());
        }
    }
}
