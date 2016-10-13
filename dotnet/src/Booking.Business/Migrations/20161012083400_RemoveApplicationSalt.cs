using Microsoft.EntityFrameworkCore.Migrations;

namespace Booking.Business.Migrations
{
    public partial class RemoveApplicationSalt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                schema: "booking",
                table: "OAuth.Applications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                schema: "booking",
                table: "OAuth.Applications",
                nullable: true);
        }
    }
}
