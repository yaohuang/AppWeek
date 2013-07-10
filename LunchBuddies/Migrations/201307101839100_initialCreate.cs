namespace LunchBuddies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Interests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LunchRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MeetingPlace = c.String(nullable: false),
                        DateTimeCreated = c.DateTime(nullable: false, storeType: "datetime2"),
                        DateTimeRequest = c.DateTime(nullable: false, storeType: "datetime2"),
                        Subject = c.String(),
                        Creator_Email = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Creator_Email)
                .Index(t => t.Creator_Email);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        Password = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Building = c.String(nullable: false),
                        Office = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Email);
            
            CreateTable(
                "dbo.UserLunchRequests",
                c => new
                    {
                        LunchRequestId = c.Int(nullable: false),
                        UserEmail = c.String(nullable: false, maxLength: 128),
                        LunchStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LunchRequestId, t.UserEmail })
                .ForeignKey("dbo.Users", t => t.UserEmail)
                .ForeignKey("dbo.LunchRequests", t => t.LunchRequestId)
                .Index(t => t.UserEmail)
                .Index(t => t.LunchRequestId);
            
            CreateTable(
                "dbo.UserLunchTimes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DayOfWeek = c.Int(nullable: false),
                        BeginTime = c.Time(nullable: false),
                        EndTime = c.Time(nullable: false),
                        User_Email = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Email)
                .Index(t => t.User_Email);
            
            CreateTable(
                "dbo.UserPictures",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Picture = c.Binary(),
                        Message = c.String(),
                        User_Email = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Email)
                .Index(t => t.User_Email);
            
            CreateTable(
                "dbo.LunchRequestInterests",
                c => new
                    {
                        LunchRequest_Id = c.Int(nullable: false),
                        Interest_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LunchRequest_Id, t.Interest_Id })
                .ForeignKey("dbo.LunchRequests", t => t.LunchRequest_Id, cascadeDelete: true)
                .ForeignKey("dbo.Interests", t => t.Interest_Id, cascadeDelete: true)
                .Index(t => t.LunchRequest_Id)
                .Index(t => t.Interest_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPictures", "User_Email", "dbo.Users");
            DropForeignKey("dbo.UserLunchTimes", "User_Email", "dbo.Users");
            DropForeignKey("dbo.LunchRequestInterests", "Interest_Id", "dbo.Interests");
            DropForeignKey("dbo.LunchRequestInterests", "LunchRequest_Id", "dbo.LunchRequests");
            DropForeignKey("dbo.UserLunchRequests", "LunchRequestId", "dbo.LunchRequests");
            DropForeignKey("dbo.LunchRequests", "Creator_Email", "dbo.Users");
            DropForeignKey("dbo.UserLunchRequests", "UserEmail", "dbo.Users");
            DropIndex("dbo.UserPictures", new[] { "User_Email" });
            DropIndex("dbo.UserLunchTimes", new[] { "User_Email" });
            DropIndex("dbo.LunchRequestInterests", new[] { "Interest_Id" });
            DropIndex("dbo.LunchRequestInterests", new[] { "LunchRequest_Id" });
            DropIndex("dbo.UserLunchRequests", new[] { "LunchRequestId" });
            DropIndex("dbo.LunchRequests", new[] { "Creator_Email" });
            DropIndex("dbo.UserLunchRequests", new[] { "UserEmail" });
            DropTable("dbo.LunchRequestInterests");
            DropTable("dbo.UserPictures");
            DropTable("dbo.UserLunchTimes");
            DropTable("dbo.UserLunchRequests");
            DropTable("dbo.Users");
            DropTable("dbo.LunchRequests");
            DropTable("dbo.Interests");
        }
    }
}
