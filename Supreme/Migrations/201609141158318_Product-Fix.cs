namespace Supreme.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductPrices", "sku", c => c.String());
            AddColumn("dbo.OrderProducts", "sku", c => c.String());
            DropColumn("dbo.Products", "sku");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "sku", c => c.String());
            DropColumn("dbo.OrderProducts", "sku");
            DropColumn("dbo.ProductPrices", "sku");
        }
    }
}
