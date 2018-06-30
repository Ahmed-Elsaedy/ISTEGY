namespace ISTEGY.Sales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        StoreId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreId, t.ProductId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransactionDetails",
                c => new
                    {
                        TranId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TranId, t.ProductId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.TranId, cascadeDelete: true)
                .Index(t => t.TranId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        VoucherSerial = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VoucherSerial)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Inventories", "ProductId", "dbo.Products");
            DropForeignKey("dbo.TransactionDetails", "TranId", "dbo.Transactions");
            DropForeignKey("dbo.Transactions", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.TransactionDetails", "ProductId", "dbo.Products");
            DropIndex("dbo.Transactions", new[] { "StoreId" });
            DropIndex("dbo.TransactionDetails", new[] { "ProductId" });
            DropIndex("dbo.TransactionDetails", new[] { "TranId" });
            DropIndex("dbo.Inventories", new[] { "ProductId" });
            DropIndex("dbo.Inventories", new[] { "StoreId" });
            DropTable("dbo.Stores");
            DropTable("dbo.Transactions");
            DropTable("dbo.TransactionDetails");
            DropTable("dbo.Products");
            DropTable("dbo.Inventories");
        }
    }
}
