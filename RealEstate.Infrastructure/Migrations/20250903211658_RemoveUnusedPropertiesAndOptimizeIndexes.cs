using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedPropertiesAndOptimizeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_properties_area_lot_price",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_hoa_fee",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_hoa_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_lot_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_lot_size",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "hoa_fee",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "last_sold_price",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "lot_size_sqft",
                table: "properties");

            migrationBuilder.CreateIndex(
                name: "idx_properties_area_range_opt",
                table: "properties",
                columns: new[] { "is_published", "listing_status", "area_sqft", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_bathrooms_range_opt",
                table: "properties",
                columns: new[] { "is_published", "listing_status", "bathrooms", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_bedrooms_range_opt",
                table: "properties",
                columns: new[] { "is_published", "listing_status", "bedrooms", "price" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_properties_area_range_opt",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_bathrooms_range_opt",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_bedrooms_range_opt",
                table: "properties");

            migrationBuilder.AddColumn<decimal>(
                name: "hoa_fee",
                table: "properties",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "last_sold_price",
                table: "properties",
                type: "decimal(14,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lot_size_sqft",
                table: "properties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_properties_area_lot_price",
                table: "properties",
                columns: new[] { "area_sqft", "lot_size_sqft", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_hoa_fee",
                table: "properties",
                column: "hoa_fee");

            migrationBuilder.CreateIndex(
                name: "idx_properties_hoa_price_range",
                table: "properties",
                columns: new[] { "hoa_fee", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_lot_price_range",
                table: "properties",
                columns: new[] { "lot_size_sqft", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_lot_size",
                table: "properties",
                column: "lot_size_sqft");
        }
    }
}
