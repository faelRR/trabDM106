namespace dm106.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajustesGeral : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        quantidade = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userEmail = c.String(),
                        userName = c.String(),
                        dateOrder = c.DateTime(nullable: false),
                        dateOrderDelivery = c.DateTime(nullable: false),
                        statusOrder = c.String(),
                        priceOrder = c.Decimal(nullable: false, precision: 18, scale: 2),
                        weightOrder = c.Decimal(nullable: false, precision: 18, scale: 2),
                        priceFreight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Products", "preco", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "peso", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "altura", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "largura", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "comprimento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "diametro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            AlterColumn("dbo.Products", "diametro", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "comprimento", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "largura", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "altura", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "peso", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "preco", c => c.Single(nullable: false));
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
        }
    }
}
