using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class Columns_PwdSenior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPregnant",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPwd",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSenior",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPregnant",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPwd",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSenior",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPregnant",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsPwd",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsSenior",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AllowPregnant",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AllowPwd",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AllowSenior",
                table: "Products");
        }
    }
}
