using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class AppointmentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentInformations_Products_ProductsId",
                table: "AppointmentInformations");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentInformations_ProductsId",
                table: "AppointmentInformations");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "AppointmentInformations",
                newName: "ProductId");

            migrationBuilder.AddColumn<int>(
                name: "PatientInformationId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HmoReference",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "AppointmentInformations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWalkIn",
                table: "AppointmentInformations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PatientInformationId",
                table: "AppointmentInformations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "AppointmentInformations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PatientInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPwd = table.Column<bool>(type: "bit", nullable: false),
                    IsSenior = table.Column<bool>(type: "bit", nullable: false),
                    IsPregnant = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInformations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_PatientInformationId",
                table: "UserProfiles",
                column: "PatientInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_PatientInformations_PatientInformationId",
                table: "UserProfiles",
                column: "PatientInformationId",
                principalTable: "PatientInformations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_PatientInformations_PatientInformationId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "PatientInformations");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_PatientInformationId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PatientInformationId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "HmoReference",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "IsWalkIn",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "PatientInformationId",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "AppointmentInformations");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "AppointmentInformations");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "AppointmentInformations",
                newName: "ProductsId");

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "AppointmentInformations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentInformations_ProductsId",
                table: "AppointmentInformations",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentInformations_Products_ProductsId",
                table: "AppointmentInformations",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
