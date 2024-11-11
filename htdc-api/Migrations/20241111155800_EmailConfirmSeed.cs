using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class EmailConfirmSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                column: "EmailConfirmed",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                column: "EmailConfirmed",
                value: false);
        }
    }
}
