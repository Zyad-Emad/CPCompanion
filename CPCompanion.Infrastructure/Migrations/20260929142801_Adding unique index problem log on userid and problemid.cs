using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPCompanion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addinguniqueindexproblemlogonuseridandproblemid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProblemLogs_UserId",
                table: "ProblemLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ProblemId",
                table: "ProblemLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemLogs_UserId_ProblemId",
                table: "ProblemLogs",
                columns: new[] { "UserId", "ProblemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProblemLogs_UserId_ProblemId",
                table: "ProblemLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ProblemId",
                table: "ProblemLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemLogs_UserId",
                table: "ProblemLogs",
                column: "UserId");
        }
    }
}
