using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ArcaiProject.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AcademicPeriods",
                columns: new[] { "Id", "EndDate", "PeriodName", "StartDate" },
                values: new object[,]
                {
                    { 1, null, "2023-2024 Fall", null },
                    { 2, null, "2023-2024 Spring", null },
                    { 3, null, "2024-2025 Fall", null }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CourseCode", "CourseName", "Department" },
                values: new object[,]
                {
                    { 1, "CEN403", "Software Design", "Computer Engineering" },
                    { 2, "MT101", "Calculus I", "Basic Sciences" },
                    { 3, "CEN305", "Database Systems", "Computer Engineering" }
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Midterm (ara sınav) exam papers.", "Midterm Exam Paper" },
                    { 2, "End-of-term (final) exam papers.", "Final Exam Paper" },
                    { 3, "Student internship reports and logbooks.", "Internship Report" },
                    { 4, "Official faculty administrative documents.", "Administrative Document" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Cabinet", "FriendlyName", "Room", "Shelf" },
                values: new object[,]
                {
                    { 1, "Cabinet A", "Room 205 - Cabinet A - Shelf 1", "Room 205", "Shelf 1" },
                    { 2, "Cabinet A", "Room 205 - Cabinet A - Shelf 2", "Room 205", "Shelf 2" },
                    { 3, "Cabinet B", "Room 205 - Cabinet B - Shelf 1", "Room 205", "Shelf 1" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Theoretical" },
                    { 2, "Practical" },
                    { 3, "Project" },
                    { 4, "MultipleChoice" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "secretary@arcai.com", "Admin", "Secretary", "AQAAAAIAAYagAAAAENn+B2BvDgeN3A3S/jAfXqfA+kQ6/bA8qTLcOhmfI4YqKE1XlK0/j8H+c0D+A9p+QQ==", "Admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "john.smith@arcai.com", "Prof. John", "Smith", "AQAAAAIAAYagAAAAEMp/yC0m8Cg1Q2bH8V/4l1EK1fB/iYqSKc2iBCcBOy1F0Pq+Yqf/QYQ3aN6xXqGvIQ==", "Professor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AcademicPeriods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AcademicPeriods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AcademicPeriods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
