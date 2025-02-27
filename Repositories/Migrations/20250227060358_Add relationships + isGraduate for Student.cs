using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddrelationshipsisGraduateforStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Must drop foreign key before drop index
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudent_Student_StudentId",
                table: "ProjectStudent");
            
            migrationBuilder.DropIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent");
            
            // Create new foreign key and new index
            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudent_Student_StudentId",
                table: "ProjectStudent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGraduated",
                table: "Student",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            

            migrationBuilder.CreateIndex(
                name: "IX_Project_FacultyId",
                table: "Project",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Faculty_FacultyId",
                table: "Project",
                column: "FacultyId",
                principalTable: "Faculty",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Faculty_FacultyId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent");

            migrationBuilder.DropIndex(
                name: "IX_Project_FacultyId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "IsGraduated",
                table: "Student");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent",
                column: "StudentId");
        }
    }
}
