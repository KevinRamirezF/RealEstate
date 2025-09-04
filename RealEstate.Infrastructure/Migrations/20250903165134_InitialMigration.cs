using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "owners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    external_code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    email = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    photo_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    address_line = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    city = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    state = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    postal_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    row_version = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_owners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owner_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code_internal = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    property_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    year_built = table.Column<short>(type: "smallint", nullable: true),
                    bedrooms = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    bathrooms = table.Column<decimal>(type: "decimal(4,1)", nullable: false, defaultValue: 0.0m),
                    parking_spaces = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    area_sqft = table.Column<int>(type: "int", nullable: true),
                    lot_size_sqft = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(14,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    hoa_fee = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    address_line = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    city = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    state = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    lat = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    lng = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    listing_status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "ACTIVE"),
                    listing_date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CAST(GETDATE() AS DATE)"),
                    last_sold_price = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    is_featured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    row_version = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.Id);
                    table.CheckConstraint("CK_properties_bathrooms", "bathrooms >= 0");
                    table.CheckConstraint("CK_properties_bedrooms", "bedrooms >= 0");
                    table.CheckConstraint("CK_properties_price", "price >= 0");
                    table.ForeignKey(
                        name: "FK_properties_owners_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "property_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    property_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    storage_provider = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "S3"),
                    alt_text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    is_primary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    sort_order = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    checksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    row_version = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_images_properties_property_id",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "property_traces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    property_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    event_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    event_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    actor_name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    old_value = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    new_value = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    tax_amount = table.Column<decimal>(type: "decimal(14,2)", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_traces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_traces_properties_property_id",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_owners_active",
                table: "owners",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "idx_owners_email",
                table: "owners",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_owners_external_code",
                table: "owners",
                column: "external_code",
                unique: true,
                filter: "[external_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_owners_location",
                table: "owners",
                columns: new[] { "state", "city" });

            migrationBuilder.CreateIndex(
                name: "idx_owners_name",
                table: "owners",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "idx_properties_area",
                table: "properties",
                column: "area_sqft");

            migrationBuilder.CreateIndex(
                name: "idx_properties_bedrooms",
                table: "properties",
                column: "bedrooms");

            migrationBuilder.CreateIndex(
                name: "idx_properties_code_internal",
                table: "properties",
                column: "code_internal",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_properties_created_at",
                table: "properties",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_properties_featured",
                table: "properties",
                column: "is_featured");

            migrationBuilder.CreateIndex(
                name: "idx_properties_geo",
                table: "properties",
                columns: new[] { "state", "city", "postal_code" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_owner",
                table: "properties",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "idx_properties_price",
                table: "properties",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "idx_properties_published_status",
                table: "properties",
                columns: new[] { "is_published", "listing_status" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_type_status",
                table: "properties",
                columns: new[] { "property_type", "listing_status" });

            migrationBuilder.CreateIndex(
                name: "idx_properties_year_built",
                table: "properties",
                column: "year_built");

            migrationBuilder.CreateIndex(
                name: "idx_property_images_primary",
                table: "property_images",
                columns: new[] { "property_id", "is_primary" });

            migrationBuilder.CreateIndex(
                name: "idx_property_images_property",
                table: "property_images",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "idx_property_traces_property",
                table: "property_traces",
                columns: new[] { "property_id", "event_date" });

            migrationBuilder.CreateIndex(
                name: "idx_property_traces_type",
                table: "property_traces",
                column: "event_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "property_images");

            migrationBuilder.DropTable(
                name: "property_traces");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "owners");
        }
    }
}
