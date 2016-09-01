namespace Supreme.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProductPrices", "productId");
            AddForeignKey("dbo.ProductPrices", "productId", "dbo.Products", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPrices", "productId", "dbo.Products");
            DropIndex("dbo.ProductPrices", new[] { "productId" });
        }
    }
}
