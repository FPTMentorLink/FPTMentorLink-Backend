using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsAcceptedinMentoringProposalwithstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "MentoringProposal");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MentoringProposal",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MentoringProposal");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "MentoringProposal",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
