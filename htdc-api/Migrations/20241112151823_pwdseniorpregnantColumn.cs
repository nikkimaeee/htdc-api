using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class pwdseniorpregnantColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPregnant",
                table: "AppointmentInformations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPwd",
                table: "AppointmentInformations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSenior",
                table: "AppointmentInformations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPregnant",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "IsPwd",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "IsSenior",
                table: "AppointmentInformations");
        }
    }
}
