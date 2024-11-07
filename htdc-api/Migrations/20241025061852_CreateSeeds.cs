using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace htdc_api.Migrations
{
    public partial class CreateSeeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppointmentTimes",
                columns: new[] { "Id", "DateCreated", "DateUpdated", "From", "IsActive", "Name", "To" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "09:00:00", true, "09:00am - 10:00am", "10:00:00" },
                    { 2, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "10:00:00", true, "10:00am - 11:00am", "11:00:00" },
                    { 3, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "11:00:00", true, "11:00am - 12:00pm", "12:00:00" },
                    { 4, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "12:00:00", true, "12:00pm - 01:00pm", "13:00:00" },
                    { 5, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "13:00:00", true, "01:00pm - 02:00pm", "14:00:00" },
                    { 6, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "14:00:00", true, "02:00pm - 03:00pm", "15:00:00" },
                    { 7, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "15:00:00", true, "03:00pm - 04:00pm", "16:00:00" },
                    { 8, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "16:00:00", true, "04:00pm - 05:00pm", "17:00:00" },
                    { 9, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "17:00:00", true, "05:00pm - 06:00pm", "18:00:00" },
                    { 10, new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679), null, "18:00:00", true, "06:00pm - 07:00pm", "19:00:00" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AppointmentTimes",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
