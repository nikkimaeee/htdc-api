using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class CreateColumn_MilitaryTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MilitaryTime",
                table: "AppointmentTimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 1,
                column: "MilitaryTime",
                value: 9);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 2,
                column: "MilitaryTime",
                value: 10);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 3,
                column: "MilitaryTime",
                value: 11);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 4,
                column: "MilitaryTime",
                value: 12);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 5,
                column: "MilitaryTime",
                value: 13);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 6,
                column: "MilitaryTime",
                value: 14);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 7,
                column: "MilitaryTime",
                value: 15);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 8,
                column: "MilitaryTime",
                value: 16);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 9,
                column: "MilitaryTime",
                value: 17);

            migrationBuilder.UpdateData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 10,
                column: "MilitaryTime",
                value: 18);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MilitaryTime",
                table: "AppointmentTimes");
        }
    }
}
