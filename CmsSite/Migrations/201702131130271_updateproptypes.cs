namespace CmsSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproptypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CmsProperties", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CmsProperties", "Type", c => c.String());
        }
    }
}
