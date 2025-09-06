using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingStructureToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_properties_price",
                table: "properties");

            migrationBuilder.RenameColumn(
                name: "tax_amount",
                table: "property_traces",
                newName: "old_total_price");

            migrationBuilder.RenameColumn(
                name: "old_value",
                table: "property_traces",
                newName: "old_tax_amount");

            migrationBuilder.RenameColumn(
                name: "new_value",
                table: "property_traces",
                newName: "old_price_base");

            migrationBuilder.AddColumn<decimal>(
                name: "base_price",
                table: "properties",
                type: "decimal(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "properties",
                type: "decimal(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_properties_base_price",
                table: "properties",
                sql: "base_price >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_properties_price",
                table: "properties",
                sql: "price >= 0 AND price = base_price + tax_amount");

            migrationBuilder.AddCheckConstraint(
                name: "CK_properties_tax_amount",
                table: "properties",
                sql: "tax_amount >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_properties_base_price",
                table: "properties");

            migrationBuilder.DropCheckConstraint(
                name: "CK_properties_price",
                table: "properties");

            migrationBuilder.DropCheckConstraint(
                name: "CK_properties_tax_amount",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "base_price",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "properties");

            migrationBuilder.RenameColumn(
                name: "old_total_price",
                table: "property_traces",
                newName: "tax_amount");

            migrationBuilder.RenameColumn(
                name: "old_tax_amount",
                table: "property_traces",
                newName: "old_value");

            migrationBuilder.RenameColumn(
                name: "old_price_base",
                table: "property_traces",
                newName: "new_value");

            migrationBuilder.AddCheckConstraint(
                name: "CK_properties_price",
                table: "properties",
                sql: "price >= 0");
        }
    }
}
