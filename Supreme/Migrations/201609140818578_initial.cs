namespace Supreme.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accountants",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user_id = c.String(nullable: false),
                        profileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Profiles", t => t.profileId, cascadeDelete: true)
                .Index(t => t.profileId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        orderid = c.Int(nullable: false),
                        accountant_id = c.Int(nullable: false),
                        status = c.String(),
                        invoiceNumber = c.String(),
                        account_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Accountants", t => t.account_id)
                .ForeignKey("dbo.Orders", t => t.orderid, cascadeDelete: true)
                .Index(t => t.orderid)
                .Index(t => t.account_id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orderNumber = c.String(),
                        invoiceNumber = c.String(),
                        date = c.DateTime(nullable: false),
                        salesRepId = c.Int(nullable: false),
                        customerId = c.Int(nullable: false),
                        status = c.String(),
                        branchId = c.Int(nullable: false),
                        warehouseLocation = c.String(),
                        price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.customerId, cascadeDelete: true)
                .ForeignKey("dbo.SalesReps", t => t.salesRepId, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.branchId, cascadeDelete: true)
                .Index(t => t.salesRepId)
                .Index(t => t.customerId)
                .Index(t => t.branchId);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        customerId = c.Int(nullable: false),
                        name = c.String(nullable: false),
                        address = c.String(),
                        location = c.String(),
                        telephone = c.String(),
                        telephone2 = c.String(),
                        branchManager = c.String(),
                        monthlyBudget = c.Int(nullable: false),
                        email = c.String(),
                        regionId = c.Int(nullable: false),
                        salesRepId = c.Int(nullable: false),
                        merchantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.customerId, cascadeDelete: false)
                .ForeignKey("dbo.Merchants", t => t.merchantId, cascadeDelete: false)
                .ForeignKey("dbo.SalesReps", t => t.salesRepId, cascadeDelete: false)
                .Index(t => t.customerId)
                .Index(t => t.salesRepId)
                .Index(t => t.merchantId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tradingName = c.String(nullable: false),
                        reference = c.String(),
                        registrationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ProductPrices",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        productId = c.Int(nullable: false),
                        customerId = c.Int(nullable: false),
                        amount = c.Double(nullable: false),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Products", t => t.productId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.customerId, cascadeDelete: true)
                .Index(t => t.productId)
                .Index(t => t.customerId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        sku = c.String(),
                        description = c.String(),
                        type = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Merchants",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user_id = c.String(),
                        profileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Profiles", t => t.profileId, cascadeDelete: true)
                .Index(t => t.profileId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userid = c.String(nullable: false),
                        firstname = c.String(),
                        lastname = c.String(),
                        middlename = c.String(),
                        email = c.String(nullable: false),
                        phone = c.String(),
                        position = c.String(nullable: false),
                        address = c.String(),
                        town = c.String(),
                        status = c.String(),
                        profile_pic = c.String(),
                        registrationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SalesReps",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userid = c.String(),
                        profileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Profiles", t => t.profileId, cascadeDelete: true)
                .Index(t => t.profileId);
            
            CreateTable(
                "dbo.OrderProducts",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orderId = c.Int(nullable: false),
                        productId = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                        price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Products", t => t.productId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.orderId, cascadeDelete: true)
                .Index(t => t.orderId)
                .Index(t => t.productId);
            
            CreateTable(
                "dbo.Administrators",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        profileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Profiles", t => t.profileId, cascadeDelete: true)
                .Index(t => t.profileId);
            
            CreateTable(
                "dbo.DeliveryNotes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        deliveryNoteNumber = c.String(),
                        date = c.DateTime(nullable: false),
                        orderId = c.Int(nullable: false),
                        driverId = c.Int(nullable: false),
                        status = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Drivers", t => t.driverId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.orderId, cascadeDelete: true)
                .Index(t => t.orderId)
                .Index(t => t.driverId);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        profileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Profiles", t => t.profileId, cascadeDelete: false)
                .Index(t => t.profileId);
            
            CreateTable(
                "dbo.Dispatches",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orderId = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        stockManagerId = c.Int(nullable: false),
                        status = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Orders", t => t.orderId, cascadeDelete: true)
                .ForeignKey("dbo.StockManagers", t => t.stockManagerId, cascadeDelete: true)
                .Index(t => t.orderId)
                .Index(t => t.stockManagerId);
            
            CreateTable(
                "dbo.StockManagers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        profileId = c.Int(nullable: false),
                        warehouseLocation = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Profiles", t => t.profileId, cascadeDelete: false)
                .Index(t => t.profileId);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Returns",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orderId = c.Int(nullable: false),
                        stockManagerId = c.Int(nullable: false),
                        driverId = c.Int(nullable: false),
                        status = c.String(),
                        reason = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Drivers", t => t.driverId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.orderId, cascadeDelete: true)
                .ForeignKey("dbo.StockManagers", t => t.stockManagerId, cascadeDelete: true)
                .Index(t => t.orderId)
                .Index(t => t.stockManagerId)
                .Index(t => t.driverId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Returns", "stockManagerId", "dbo.StockManagers");
            DropForeignKey("dbo.Returns", "orderId", "dbo.Orders");
            DropForeignKey("dbo.Returns", "driverId", "dbo.Drivers");
            DropForeignKey("dbo.StockManagers", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.Dispatches", "stockManagerId", "dbo.StockManagers");
            DropForeignKey("dbo.Dispatches", "orderId", "dbo.Orders");
            DropForeignKey("dbo.DeliveryNotes", "orderId", "dbo.Orders");
            DropForeignKey("dbo.Drivers", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.DeliveryNotes", "driverId", "dbo.Drivers");
            DropForeignKey("dbo.Administrators", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.Accountants", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.Invoices", "orderid", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "orderId", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "productId", "dbo.Products");
            DropForeignKey("dbo.Orders", "branchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "salesRepId", "dbo.SalesReps");
            DropForeignKey("dbo.SalesReps", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.Orders", "salesRepId", "dbo.SalesReps");
            DropForeignKey("dbo.Branches", "merchantId", "dbo.Merchants");
            DropForeignKey("dbo.Merchants", "profileId", "dbo.Profiles");
            DropForeignKey("dbo.ProductPrices", "customerId", "dbo.Customers");
            DropForeignKey("dbo.ProductPrices", "productId", "dbo.Products");
            DropForeignKey("dbo.Orders", "customerId", "dbo.Customers");
            DropForeignKey("dbo.Branches", "customerId", "dbo.Customers");
            DropForeignKey("dbo.Invoices", "account_id", "dbo.Accountants");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Returns", new[] { "driverId" });
            DropIndex("dbo.Returns", new[] { "stockManagerId" });
            DropIndex("dbo.Returns", new[] { "orderId" });
            DropIndex("dbo.StockManagers", new[] { "profileId" });
            DropIndex("dbo.Dispatches", new[] { "stockManagerId" });
            DropIndex("dbo.Dispatches", new[] { "orderId" });
            DropIndex("dbo.Drivers", new[] { "profileId" });
            DropIndex("dbo.DeliveryNotes", new[] { "driverId" });
            DropIndex("dbo.DeliveryNotes", new[] { "orderId" });
            DropIndex("dbo.Administrators", new[] { "profileId" });
            DropIndex("dbo.OrderProducts", new[] { "productId" });
            DropIndex("dbo.OrderProducts", new[] { "orderId" });
            DropIndex("dbo.SalesReps", new[] { "profileId" });
            DropIndex("dbo.Merchants", new[] { "profileId" });
            DropIndex("dbo.ProductPrices", new[] { "customerId" });
            DropIndex("dbo.ProductPrices", new[] { "productId" });
            DropIndex("dbo.Branches", new[] { "merchantId" });
            DropIndex("dbo.Branches", new[] { "salesRepId" });
            DropIndex("dbo.Branches", new[] { "customerId" });
            DropIndex("dbo.Orders", new[] { "branchId" });
            DropIndex("dbo.Orders", new[] { "customerId" });
            DropIndex("dbo.Orders", new[] { "salesRepId" });
            DropIndex("dbo.Invoices", new[] { "account_id" });
            DropIndex("dbo.Invoices", new[] { "orderid" });
            DropIndex("dbo.Accountants", new[] { "profileId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Returns");
            DropTable("dbo.Regions");
            DropTable("dbo.StockManagers");
            DropTable("dbo.Dispatches");
            DropTable("dbo.Drivers");
            DropTable("dbo.DeliveryNotes");
            DropTable("dbo.Administrators");
            DropTable("dbo.OrderProducts");
            DropTable("dbo.SalesReps");
            DropTable("dbo.Profiles");
            DropTable("dbo.Merchants");
            DropTable("dbo.Products");
            DropTable("dbo.ProductPrices");
            DropTable("dbo.Customers");
            DropTable("dbo.Branches");
            DropTable("dbo.Orders");
            DropTable("dbo.Invoices");
            DropTable("dbo.Accountants");
        }
    }
}
