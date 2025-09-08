using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RowVersionByteArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing int row_version columns
            migrationBuilder.DropColumn(
                name: "row_version",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "owners");

            // Add new rowversion columns
            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "properties",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "owners",
                type: "rowversion",
                rowVersion: true,
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the rowversion columns
            migrationBuilder.DropColumn(
                name: "row_version",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "owners");

            // Restore the original int row_version columns
            migrationBuilder.AddColumn<int>(
                name: "row_version",
                table: "properties",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "row_version",
                table: "owners",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }
    }
}
