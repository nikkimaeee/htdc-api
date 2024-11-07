using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class UpdateAppointmentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentInformations_AppointmentTimes_AppointmentTimeId",
                table: "AppointmentInformations");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentInformations_AppointmentTimeId",
                table: "AppointmentInformations");

            migrationBuilder.RenameColumn(
                name: "AppointmentTimeId",
                table: "AppointmentInformations",
                newName: "AppointmentDuration");

            migrationBuilder.AddColumn<string>(
                name: "AppointmentTimeIds",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentTimeIds",
                table: "AppointmentInformations");

            migrationBuilder.RenameColumn(
                name: "AppointmentDuration",
                table: "AppointmentInformations",
                newName: "AppointmentTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentInformations_AppointmentTimeId",
                table: "AppointmentInformations",
                column: "AppointmentTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentInformations_AppointmentTimes_AppointmentTimeId",
                table: "AppointmentInformations",
                column: "AppointmentTimeId",
                principalTable: "AppointmentTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
