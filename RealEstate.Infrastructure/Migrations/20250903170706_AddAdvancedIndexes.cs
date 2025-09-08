using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "idx_properties_published_status",
                table: "properties",
                newName: "idx_properties_published_listing_status");

            migrationBuilder.CreateIndex(
                name: "idx_property_traces_complete",
                table: "property_traces",
                columns: new[] { "property_id", "event_type", "event_date" });

            migrationBuilder.CreateIndex(
                name: "idx_property_traces_timeline",
                table: "property_traces",
                columns: new[] { "event_date", "event_type" });

            migrationBuilder.CreateIndex(
                name: "idx_property_images_enabled",
                table: "property_images",
                column: "enabled");

            migrationBuilder.CreateIndex(
                name: "idx_property_images_sorting",
                table: "property_images",
                columns: new[] { "property_id", "sort_order", "enabled" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_area_lot_price",
                table: "properties",
                columns: new[] { "area_sqft", "lot_size_sqft", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_area_price_range",
                table: "properties",
                columns: new[] { "area_sqft", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_bathrooms",
                table: "properties",
                column: "bathrooms");

            migrationBuilder.CreateIndex(
                name: "idx_properties_bathrooms_price_range",
                table: "properties",
                columns: new[] { "bathrooms", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_bedrooms_price_range",
                table: "properties",
                columns: new[] { "bedrooms", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_beds_baths_price",
                table: "properties",
                columns: new[] { "bedrooms", "bathrooms", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_coordinates",
                table: "properties",
                columns: new[] { "lat", "lng" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_full_range_search",
                table: "properties",
                columns: new[] { "property_type", "bedrooms", "bathrooms", "price", "area_sqft" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_geospatial_published",
                table: "properties",
                columns: new[] { "lat", "lng", "is_published", "listing_status" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_hoa_fee",
                table: "properties",
                column: "hoa_fee");

            migrationBuilder.CreateIndex(
                name: "idx_properties_hoa_price_range",
                table: "properties",
                columns: new[] { "hoa_fee", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_listing_date",
                table: "properties",
                column: "listing_date");

            migrationBuilder.CreateIndex(
                name: "idx_properties_location_price",
                table: "properties",
                columns: new[] { "state", "city", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_location_type_specs",
                table: "properties",
                columns: new[] { "state", "city", "property_type", "price", "bedrooms" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_lot_price_range",
                table: "properties",
                columns: new[] { "lot_size_sqft", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_lot_size",
                table: "properties",
                column: "lot_size_sqft");

            migrationBuilder.CreateIndex(
                name: "idx_properties_name",
                table: "properties",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "idx_properties_price_listing",
                table: "properties",
                columns: new[] { "price", "listing_date" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_published_featured",
                table: "properties",
                columns: new[] { "is_published", "is_featured", "listing_date" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_published_sorted",
                table: "properties",
                columns: new[] { "is_published", "price", "listing_date", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_search_ultimate",
                table: "properties",
                columns: new[] { "is_published", "property_type", "listing_status", "state", "city", "price" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_type_price_bedrooms",
                table: "properties",
                columns: new[] { "property_type", "price", "bedrooms" });

            migrationBuilder.CreateIndex(
                name: "idx_owners_active_created",
                table: "owners",
                columns: new[] { "is_active", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_owners_created_at",
                table: "owners",
                column: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_property_traces_complete",
                table: "property_traces");

            migrationBuilder.DropIndex(
                name: "idx_property_traces_timeline",
                table: "property_traces");

            migrationBuilder.DropIndex(
                name: "idx_property_images_enabled",
                table: "property_images");

            migrationBuilder.DropIndex(
                name: "idx_property_images_sorting",
                table: "property_images");

            migrationBuilder.DropIndex(
                name: "idx_properties_area_lot_price",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_area_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_bathrooms",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_bathrooms_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_bedrooms_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_beds_baths_price",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_coordinates",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_full_range_search",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_geospatial_published",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_hoa_fee",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_hoa_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_listing_date",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_location_price",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_location_type_specs",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_lot_price_range",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_lot_size",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_name",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_price_listing",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_published_featured",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_published_sorted",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_search_ultimate",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_properties_type_price_bedrooms",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "idx_owners_active_created",
                table: "owners");

            migrationBuilder.DropIndex(
                name: "idx_owners_created_at",
                table: "owners");

            migrationBuilder.RenameIndex(
                name: "idx_properties_published_listing_status",
                table: "properties",
                newName: "idx_properties_published_status");
        }
    }
}
