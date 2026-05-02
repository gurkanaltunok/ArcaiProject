using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArcaiProject.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "Title", "Status", "Description", "CreatedAt", "DocumentTypeId", "LocationId", "AddedByUserId", "CourseId", "AcademicPeriodId" },
                values: new object[,]
                {
                    { 100, "CEN403 Midterm Exam 2024", "Available", "Software Design midterm examination paper from Fall 2024", new DateTime(2024, 9, 15, 10, 0, 0, DateTimeKind.Utc), 1, 1, 1, 1, 3 },
                    { 101, "CEN403 Final Exam 2024", "Available", "Software Design final examination paper from Fall 2024", new DateTime(2024, 12, 20, 14, 0, 0, DateTimeKind.Utc), 2, 1, 1, 1, 3 },
                    { 102, "MT101 Calculus I Midterm", "CheckedOut", "Calculus I midterm exam paper", new DateTime(2024, 9, 10, 9, 0, 0, DateTimeKind.Utc), 1, 2, 1, 2, 3 },
                    { 103, "CEN305 Database Systems Final", "Available", "Database Systems final examination", new DateTime(2024, 12, 15, 11, 0, 0, DateTimeKind.Utc), 2, 3, 1, 3, 3 },
                    { 104, "John Smith Internship Report", "Available", "Summer internship report submitted by John Smith", new DateTime(2024, 8, 1, 8, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, null },
                    { 105, "Faculty Meeting Minutes - September 2024", "Available", "Official minutes from faculty meeting held in September 2024", new DateTime(2024, 9, 5, 13, 0, 0, DateTimeKind.Utc), 4, 2, 1, null, null },
                    { 106, "CEN403 Project Guidelines", "Available", "Software Design project guidelines and requirements", new DateTime(2024, 9, 1, 10, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 107, "MT101 Final Exam Solutions", "Missing", "Calculus I final exam solution key", new DateTime(2024, 12, 22, 15, 0, 0, DateTimeKind.Utc), 2, 2, 1, 2, 3 },
                    { 108, "CEN305 Database Design Assignment", "Available", "Database design assignment submission example", new DateTime(2024, 10, 15, 12, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 109, "Student Internship Report - Summer 2024", "CheckedOut", "Internship report from summer 2024 program", new DateTime(2024, 7, 20, 9, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, null },
                    { 110, "CEN403 Midterm Exam Solutions", "Available", "Solution key for Software Design midterm exam", new DateTime(2024, 9, 16, 14, 0, 0, DateTimeKind.Utc), 1, 1, 1, 1, 3 },
                    { 111, "Academic Calendar 2024-2025", "Available", "Official academic calendar for 2024-2025 academic year", new DateTime(2024, 8, 1, 8, 0, 0, DateTimeKind.Utc), 4, 2, 1, null, 3 },
                    { 112, "MT101 Quiz 1 Paper", "Available", "First quiz paper for Calculus I course", new DateTime(2024, 9, 20, 10, 0, 0, DateTimeKind.Utc), 1, 2, 1, 2, 3 },
                    { 113, "CEN305 Lab Exercise 1", "Available", "Database Systems laboratory exercise document", new DateTime(2024, 9, 5, 11, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 114, "Faculty Recruitment Guidelines", "Available", "Guidelines for faculty recruitment process", new DateTime(2024, 6, 1, 10, 0, 0, DateTimeKind.Utc), 4, 1, 1, null, null },
                    { 115, "CEN403 Final Exam Solutions", "CheckedOut", "Solution key for Software Design final exam", new DateTime(2024, 12, 21, 15, 0, 0, DateTimeKind.Utc), 2, 1, 1, 1, 3 },
                    { 116, "MT101 Midterm Exam Solutions", "Available", "Calculus I midterm exam solution key", new DateTime(2024, 9, 11, 11, 0, 0, DateTimeKind.Utc), 1, 2, 1, 2, 3 },
                    { 117, "CEN305 Project Documentation", "Available", "Database Systems project documentation template", new DateTime(2024, 10, 1, 9, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 118, "Industrial Internship Report 2024", "Available", "Sample industrial internship report from 2024", new DateTime(2024, 8, 15, 13, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, null },
                    { 119, "Department Meeting Agenda - October", "Available", "Department meeting agenda for October 2024", new DateTime(2024, 10, 1, 8, 0, 0, DateTimeKind.Utc), 4, 2, 1, null, 3 },
                    { 120, "CEN403 Assignment 1 Solutions", "Available", "Software Design assignment 1 solution examples", new DateTime(2024, 9, 25, 10, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 121, "MT101 Assignment Submission", "CheckedOut", "Calculus I assignment submission example", new DateTime(2024, 10, 5, 12, 0, 0, DateTimeKind.Utc), 4, 2, 1, 2, 3 },
                    { 122, "CEN305 Midterm Exam 2024", "Available", "Database Systems midterm examination paper", new DateTime(2024, 10, 20, 14, 0, 0, DateTimeKind.Utc), 1, 3, 1, 3, 3 },
                    { 123, "CEN305 Midterm Exam Solutions", "Available", "Database Systems midterm exam solution key", new DateTime(2024, 10, 21, 15, 0, 0, DateTimeKind.Utc), 1, 3, 1, 3, 3 },
                    { 124, "Spring 2024 Internship Reports", "Available", "Collection of internship reports from Spring 2024", new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, 2 },
                    { 125, "CEN403 Course Syllabus", "Available", "Software Design course syllabus and outline", new DateTime(2024, 8, 25, 8, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 126, "MT101 Course Materials", "Available", "Calculus I course materials and handouts", new DateTime(2024, 9, 1, 10, 0, 0, DateTimeKind.Utc), 4, 2, 1, 2, 3 },
                    { 127, "CEN305 Final Exam Solutions", "Missing", "Database Systems final exam solution key", new DateTime(2024, 12, 16, 16, 0, 0, DateTimeKind.Utc), 2, 3, 1, 3, 3 },
                    { 128, "Faculty Research Guidelines", "Available", "Guidelines for faculty research activities", new DateTime(2024, 7, 1, 11, 0, 0, DateTimeKind.Utc), 4, 1, 1, null, null },
                    { 129, "CEN403 Project Presentation Template", "Available", "Template for Software Design project presentations", new DateTime(2024, 11, 1, 10, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 130, "MT101 Quiz 2 Paper", "Available", "Second quiz paper for Calculus I course", new DateTime(2024, 10, 10, 10, 0, 0, DateTimeKind.Utc), 1, 2, 1, 2, 3 },
                    { 131, "CEN305 Database Schema Design", "CheckedOut", "Database schema design examples and templates", new DateTime(2024, 10, 10, 12, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 132, "Fall 2023 Internship Report", "Available", "Sample internship report from Fall 2023", new DateTime(2023, 12, 15, 9, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, 1 },
                    { 133, "Department Budget Report 2024", "Available", "Department budget report for fiscal year 2024", new DateTime(2024, 6, 15, 13, 0, 0, DateTimeKind.Utc), 4, 2, 1, null, null },
                    { 134, "CEN403 Lab Exercise Solutions", "Available", "Software Design lab exercise solutions", new DateTime(2024, 9, 30, 11, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 135, "MT101 Final Exam 2024", "Available", "Calculus I final examination paper from Fall 2024", new DateTime(2024, 12, 18, 14, 0, 0, DateTimeKind.Utc), 2, 2, 1, 2, 3 },
                    { 136, "CEN305 Assignment Guidelines", "Available", "Database Systems assignment guidelines and rubrics", new DateTime(2024, 9, 15, 9, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 137, "Student Internship Guidelines 2024", "Available", "Guidelines and requirements for student internships in 2024", new DateTime(2024, 7, 1, 8, 0, 0, DateTimeKind.Utc), 4, 1, 1, null, null },
                    { 138, "CEN403 Quiz 1 Paper", "CheckedOut", "First quiz paper for Software Design course", new DateTime(2024, 9, 30, 10, 0, 0, DateTimeKind.Utc), 1, 1, 1, 1, 3 },
                    { 139, "MT101 Midterm Exam 2024", "Available", "Calculus I midterm examination paper from Fall 2024", new DateTime(2024, 10, 15, 14, 0, 0, DateTimeKind.Utc), 1, 2, 1, 2, 3 },
                    { 140, "CEN305 Lab Manual", "Available", "Database Systems laboratory manual and instructions", new DateTime(2024, 9, 1, 8, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 141, "Graduate Research Internship Report", "Available", "Graduate student research internship report", new DateTime(2024, 8, 20, 10, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, null },
                    { 142, "Faculty Evaluation Criteria", "Available", "Criteria and guidelines for faculty performance evaluation", new DateTime(2024, 5, 1, 11, 0, 0, DateTimeKind.Utc), 4, 2, 1, null, null },
                    { 143, "CEN403 Final Project Requirements", "Available", "Software Design final project requirements and specifications", new DateTime(2024, 11, 15, 10, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 144, "MT101 Quiz 3 Paper", "Available", "Third quiz paper for Calculus I course", new DateTime(2024, 11, 5, 10, 0, 0, DateTimeKind.Utc), 1, 2, 1, 2, 3 },
                    { 145, "CEN305 SQL Query Examples", "Available", "Database Systems SQL query examples and practice problems", new DateTime(2024, 10, 25, 12, 0, 0, DateTimeKind.Utc), 4, 3, 1, 3, 3 },
                    { 146, "Spring 2024 Internship Evaluation", "CheckedOut", "Internship evaluation reports from Spring 2024 semester", new DateTime(2024, 5, 30, 9, 0, 0, DateTimeKind.Utc), 3, 1, 1, null, 2 },
                    { 147, "Department Policy Manual", "Available", "Comprehensive department policy manual and procedures", new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc), 4, 2, 1, null, null },
                    { 148, "CEN403 Assignment 2 Solutions", "Available", "Software Design assignment 2 solution examples", new DateTime(2024, 10, 15, 10, 0, 0, DateTimeKind.Utc), 4, 1, 1, 1, 3 },
                    { 149, "MT101 Course Evaluation Results", "Missing", "Calculus I course evaluation results and feedback", new DateTime(2024, 12, 10, 15, 0, 0, DateTimeKind.Utc), 4, 2, 1, 2, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete all sample documents (IDs 100-149)
            for (int i = 100; i <= 149; i++)
            {
                migrationBuilder.DeleteData(
                    table: "Documents",
                    keyColumn: "Id",
                    keyValue: i);
            }
        }
    }
}
