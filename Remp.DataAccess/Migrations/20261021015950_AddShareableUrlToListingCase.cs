using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShareableUrlToListingCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShareableUrl",
                table: "ListingCases",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListingCases_ShareableUrl",
                table: "ListingCases",
                column: "ShareableUrl",
                unique: true,
                filter: "[ShareableUrl] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListingCases_ShareableUrl",
                table: "ListingCases");

            migrationBuilder.DropColumn(
                name: "ShareableUrl",
                table: "ListingCases");
        }
    }
}
