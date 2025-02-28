using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Add11RelationsbetweenAccountandactors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLogs_Accounts_SetBy",
                table: "TaskLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_Accounts_Id",
                table: "Lecturers",
                column: "Id",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentors_Accounts_Id",
                table: "Mentors",
                column: "Id",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Accounts_Id",
                table: "Students",
                column: "Id",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLogs_Lecturers_SetBy",
                table: "TaskLogs",
                column: "SetBy",
                principalTable: "Lecturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_Accounts_Id",
                table: "Lecturers");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentors_Accounts_Id",
                table: "Mentors");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Accounts_Id",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskLogs_Lecturers_SetBy",
                table: "TaskLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLogs_Accounts_SetBy",
                table: "TaskLogs",
                column: "SetBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
