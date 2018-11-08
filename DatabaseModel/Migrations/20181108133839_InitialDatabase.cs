using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    BlogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "nvarchar", maxLength: 80, nullable: false),
                    Created = table.Column<DateTime>(type: "DateTime", nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.BlogId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BlogId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Posted = table.Column<DateTime>(nullable: false),
                    PostedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryDescriptions",
                columns: table => new
                {
                    LanguageId = table.Column<string>(maxLength: 3, nullable: false),
                    Description = table.Column<string>(maxLength: 80, nullable: false),
                    CountryId = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryDescriptions", x => new { x.CountryId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_CountryDescriptions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name1 = table.Column<string>(nullable: true),
                    Name2 = table.Column<string>(nullable: true),
                    Name3 = table.Column<string>(nullable: true),
                    LanguageId = table.Column<string>(nullable: true),
                    AlternativeLanguageId = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    LastUpdatedBy = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VisitingAddressId = table.Column<int>(nullable: false),
                    PostalAddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Street = table.Column<string>(nullable: true),
                    HouseNo = table.Column<int>(nullable: false),
                    Addition = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CountryId = table.Column<string>(maxLength: 2, nullable: true),
                    VisitingAddressContactId = table.Column<int>(nullable: true),
                    PostalAddressContactId = table.Column<int>(nullable: true),
                    DeliveryAddressContactId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Contacts_DeliveryAddressContactId",
                        column: x => x.DeliveryAddressContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DeliveryAddressContactId",
                table: "Addresses",
                column: "DeliveryAddressContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs",
                columns: new[] { "Created", "BlogId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PostalAddressId",
                table: "Contacts",
                column: "PostalAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_VisitingAddressId",
                table: "Contacts",
                column: "VisitingAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CountryDescriptions_LanguageId_CountryId",
                table: "CountryDescriptions",
                columns: new[] { "LanguageId", "CountryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogId",
                table: "Posts",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Addresses_PostalAddressId",
                table: "Contacts",
                column: "PostalAddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Addresses_VisitingAddressId",
                table: "Contacts",
                column: "VisitingAddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Contacts_DeliveryAddressContactId",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "CountryDescriptions");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
