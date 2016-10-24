namespace Supreme.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesLedgers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        deliveryNote_id = c.Int(nullable: false),
                        order_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.DeliveryNotes", t => t.deliveryNote_id, cascadeDelete: false)
                .ForeignKey("dbo.Orders", t => t.order_id, cascadeDelete: false)
                .Index(t => t.deliveryNote_id)
                .Index(t => t.order_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesLedgers", "order_id", "dbo.Orders");
            DropForeignKey("dbo.SalesLedgers", "deliveryNote_id", "dbo.DeliveryNotes");
            DropIndex("dbo.SalesLedgers", new[] { "order_id" });
            DropIndex("dbo.SalesLedgers", new[] { "deliveryNote_id" });
            DropTable("dbo.SalesLedgers");
        }
    }
}
