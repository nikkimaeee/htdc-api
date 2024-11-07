using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class AppointmentAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prescriptions",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppointmentAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppointmentInformationId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentAttachments_AppointmentInformations_AppointmentInformationId",
                        column: x => x.AppointmentInformationId,
                        principalTable: "AppointmentInformations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttachments_AppointmentInformationId",
                table: "AppointmentAttachments",
                column: "AppointmentInformationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentAttachments");

            migrationBuilder.DropColumn(
                name: "Prescriptions",
                table: "AppointmentInformations");
        }
    }
}
