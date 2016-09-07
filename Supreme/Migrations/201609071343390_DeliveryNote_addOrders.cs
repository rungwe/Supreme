namespace Supreme.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliveryNote_addOrders : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DeliveryNotes", "orderId");
            AddForeignKey("dbo.DeliveryNotes", "orderId", "dbo.Orders", "id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeliveryNotes", "orderId", "dbo.Orders");
            DropIndex("dbo.DeliveryNotes", new[] { "orderId" });
        }
    }
}
