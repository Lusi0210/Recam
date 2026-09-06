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
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareableUrl",
                table: "ListingCases");
        }
    }
}
