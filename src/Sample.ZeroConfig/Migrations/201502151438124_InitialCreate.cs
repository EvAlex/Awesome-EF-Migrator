namespace Sample.ZeroConfig.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        IssuerId = c.Int(nullable: false),
                        AssigneeId = c.Int(),
                        IssueTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AssigneeId)
                .ForeignKey("dbo.Users", t => t.IssuerId)
                .Index(t => t.IssuerId)
                .Index(t => t.AssigneeId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Fullname = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IssueStatusChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChangeTime = c.DateTime(nullable: false),
                        InitiatorId = c.Int(nullable: false),
                        IssueId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.InitiatorId)
                .ForeignKey("dbo.Issues", t => t.IssueId)
                .ForeignKey("dbo.IssueStatus", t => t.StatusId)
                .Index(t => t.InitiatorId)
                .Index(t => t.IssueId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.IssueStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Issues", "IssuerId", "dbo.Users");
            DropForeignKey("dbo.Issues", "AssigneeId", "dbo.Users");
            DropForeignKey("dbo.IssueStatusChanges", "StatusId", "dbo.IssueStatus");
            DropForeignKey("dbo.IssueStatusChanges", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.IssueStatusChanges", "InitiatorId", "dbo.Users");
            DropIndex("dbo.IssueStatusChanges", new[] { "StatusId" });
            DropIndex("dbo.IssueStatusChanges", new[] { "IssueId" });
            DropIndex("dbo.IssueStatusChanges", new[] { "InitiatorId" });
            DropIndex("dbo.Issues", new[] { "AssigneeId" });
            DropIndex("dbo.Issues", new[] { "IssuerId" });
            DropTable("dbo.IssueStatus");
            DropTable("dbo.IssueStatusChanges");
            DropTable("dbo.Users");
            DropTable("dbo.Issues");
        }
    }
}
