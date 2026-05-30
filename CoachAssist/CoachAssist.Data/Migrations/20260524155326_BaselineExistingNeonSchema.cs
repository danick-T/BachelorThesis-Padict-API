using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachAssist.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaselineExistingNeonSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline voor het schema dat al in Neon bestaat.
            // Nieuwe wijzigingen komen in aparte migrations na deze startpositie.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
