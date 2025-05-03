using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCurriculum.Migrations
{
    /// <inheritdoc />
    public partial class AlterRecruiterColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Technilogies",
                table: "CandidateProfiles",
                newName: "Technologies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Technologies",
                table: "CandidateProfiles",
                newName: "Technilogies");
        }
    }
}
