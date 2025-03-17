using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Modify1StudentHasManyProjectStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First drop the foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudent_Student_StudentId",
                table: "ProjectStudent");

            // Then drop the index
            migrationBuilder.DropIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent");

            // Create new non-unique index
            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent",
                column: "StudentId");

            // Recreate the foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudent_Student_StudentId",
                table: "ProjectStudent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First drop the foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudent_Student_StudentId",
                table: "ProjectStudent");

            // Then drop the index
            migrationBuilder.DropIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent");

            // Create unique index
            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudent_StudentId",
                table: "ProjectStudent",
                column: "StudentId",
                unique: true);

            // Recreate the foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudent_Student_StudentId",
                table: "ProjectStudent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
