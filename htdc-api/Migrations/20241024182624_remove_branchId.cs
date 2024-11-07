using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class remove_branchId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "UserProfiles");

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "AspNetUserId", "Avatar", "DateCreated", "DateUpdated", "FirstName", "IsActive", "LastLogin", "LastName" },
                values: new object[] { 1, "b74ddd14-6340-4840-95c2-db12554843e5", "", new DateTime(2022, 9, 19, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "Admin", true, null, "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "UserProfiles",
                type: "int",
                nullable: true);
        }
    }
}
