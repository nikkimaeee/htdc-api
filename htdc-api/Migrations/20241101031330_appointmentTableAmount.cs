using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class appointmentTableAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HmoReference",
                table: "AppointmentInformations");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "AppointmentInformations",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "AppointmentInformations");

            migrationBuilder.AddColumn<string>(
                name: "HmoReference",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
