using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booking.Business.Migrations
{
    public partial class AddOAuthApplications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuth.Applications",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RedirectAllowSubdomains = table.Column<bool>(nullable: false),
                    RedirectAllowSubpaths = table.Column<bool>(nullable: false),
                    RedirectUrl = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuth.Applications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuth.Applications",
                schema: "booking");
        }
    }
}
