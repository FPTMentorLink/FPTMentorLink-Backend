using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixStudentIdtolecturerIdinWeeklyReportFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyReportFeedback_Student_StudentId",
                table: "WeeklyReportFeedback");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "WeeklyReportFeedback",
                newName: "LecturerId");

            migrationBuilder.RenameIndex(
                name: "IX_WeeklyReportFeedback_StudentId",
                table: "WeeklyReportFeedback",
                newName: "IX_WeeklyReportFeedback_LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyReportFeedback_Lecturer_LecturerId",
                table: "WeeklyReportFeedback",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyReportFeedback_Lecturer_LecturerId",
                table: "WeeklyReportFeedback");

            migrationBuilder.RenameColumn(
                name: "LecturerId",
                table: "WeeklyReportFeedback",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_WeeklyReportFeedback_LecturerId",
                table: "WeeklyReportFeedback",
                newName: "IX_WeeklyReportFeedback_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyReportFeedback_Student_StudentId",
                table: "WeeklyReportFeedback",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
