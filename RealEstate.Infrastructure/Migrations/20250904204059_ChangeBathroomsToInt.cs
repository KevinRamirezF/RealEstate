using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBathroomsToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the check constraint first
            migrationBuilder.Sql("ALTER TABLE properties DROP CONSTRAINT CK_properties_bathrooms");
            
            migrationBuilder.AlterColumn<int>(
                name: "bathrooms",
                table: "properties",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)",
                oldDefaultValue: 0.0m);
            
            // Recreate the check constraint
            migrationBuilder.Sql("ALTER TABLE properties ADD CONSTRAINT CK_properties_bathrooms CHECK (bathrooms >= 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the check constraint first
            migrationBuilder.Sql("ALTER TABLE properties DROP CONSTRAINT CK_properties_bathrooms");
            
            migrationBuilder.AlterColumn<decimal>(
                name: "bathrooms",
                table: "properties",
                type: "decimal(4,1)",
                nullable: false,
                defaultValue: 0.0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
            
            // Recreate the check constraint
            migrationBuilder.Sql("ALTER TABLE properties ADD CONSTRAINT CK_properties_bathrooms CHECK (bathrooms >= 0)");
        }
    }
}
