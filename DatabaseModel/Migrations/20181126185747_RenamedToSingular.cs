using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class RenamedToSingular : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Contacts_DeliveryAddressContactId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Addresses_PostalAddressId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Addresses_VisitingAddressId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_CountryDescriptions_Countries_CountryId",
                table: "CountryDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldOfInterestDescriptions_FieldsOfInterest_FieldOfInterestId",
                table: "FieldOfInterestDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Blogs_BlogId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldsOfInterest",
                table: "FieldsOfInterest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldOfInterestDescriptions",
                table: "FieldOfInterestDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryDescriptions",
                table: "CountryDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "FieldsOfInterest",
                newName: "FieldOfInterest");

            migrationBuilder.RenameTable(
                name: "FieldOfInterestDescriptions",
                newName: "FieldOfInterestDescription");

            migrationBuilder.RenameTable(
                name: "CountryDescriptions",
                newName: "CountryDescription");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "Country");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "Contact");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "Blog");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_BlogId",
                table: "Post",
                newName: "IX_Post_BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_FieldOfInterestDescriptions_LanguageId_FieldOfInterestId",
                table: "FieldOfInterestDescription",
                newName: "IX_FieldOfInterestDescription_LanguageId_FieldOfInterestId");

            migrationBuilder.RenameIndex(
                name: "IX_CountryDescriptions_LanguageId_CountryId",
                table: "CountryDescription",
                newName: "IX_CountryDescription_LanguageId_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_VisitingAddressId",
                table: "Contact",
                newName: "IX_Contact_VisitingAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_PostalAddressId",
                table: "Contact",
                newName: "IX_Contact_PostalAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blog",
                newName: "IX_Blog_Created_BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_DeliveryAddressContactId",
                table: "Address",
                newName: "IX_Address_DeliveryAddressContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CountryId",
                table: "Address",
                newName: "IX_Address_CountryId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "FieldOfInterest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "FieldOfInterest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldOfInterest",
                table: "FieldOfInterest",
                column: "FieldOfInterestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldOfInterestDescription",
                table: "FieldOfInterestDescription",
                columns: new[] { "FieldOfInterestId", "LanguageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryDescription",
                table: "CountryDescription",
                columns: new[] { "CountryId", "LanguageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Country",
                table: "Country",
                column: "CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contact",
                table: "Contact",
                column: "ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blog",
                table: "Blog",
                column: "BlogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Country_CountryId",
                table: "Address",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Contact_DeliveryAddressContactId",
                table: "Address",
                column: "DeliveryAddressContactId",
                principalTable: "Contact",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Address_PostalAddressId",
                table: "Contact",
                column: "PostalAddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Address_VisitingAddressId",
                table: "Contact",
                column: "VisitingAddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryDescription_Country_CountryId",
                table: "CountryDescription",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldOfInterestDescription_FieldOfInterest_FieldOfInterestId",
                table: "FieldOfInterestDescription",
                column: "FieldOfInterestId",
                principalTable: "FieldOfInterest",
                principalColumn: "FieldOfInterestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Blog_BlogId",
                table: "Post",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Country_CountryId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Contact_DeliveryAddressContactId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Address_PostalAddressId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Address_VisitingAddressId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_CountryDescription_Country_CountryId",
                table: "CountryDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldOfInterestDescription_FieldOfInterest_FieldOfInterestId",
                table: "FieldOfInterestDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Blog_BlogId",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldOfInterestDescription",
                table: "FieldOfInterestDescription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldOfInterest",
                table: "FieldOfInterest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryDescription",
                table: "CountryDescription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Country",
                table: "Country");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contact",
                table: "Contact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blog",
                table: "Blog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "FieldOfInterest");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "FieldOfInterest");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "FieldOfInterestDescription",
                newName: "FieldOfInterestDescriptions");

            migrationBuilder.RenameTable(
                name: "FieldOfInterest",
                newName: "FieldsOfInterest");

            migrationBuilder.RenameTable(
                name: "CountryDescription",
                newName: "CountryDescriptions");

            migrationBuilder.RenameTable(
                name: "Country",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "Contact",
                newName: "Contacts");

            migrationBuilder.RenameTable(
                name: "Blog",
                newName: "Blogs");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Post_BlogId",
                table: "Posts",
                newName: "IX_Posts_BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_FieldOfInterestDescription_LanguageId_FieldOfInterestId",
                table: "FieldOfInterestDescriptions",
                newName: "IX_FieldOfInterestDescriptions_LanguageId_FieldOfInterestId");

            migrationBuilder.RenameIndex(
                name: "IX_CountryDescription_LanguageId_CountryId",
                table: "CountryDescriptions",
                newName: "IX_CountryDescriptions_LanguageId_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_VisitingAddressId",
                table: "Contacts",
                newName: "IX_Contacts_VisitingAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_PostalAddressId",
                table: "Contacts",
                newName: "IX_Contacts_PostalAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Blog_Created_BlogId",
                table: "Blogs",
                newName: "IX_Blogs_Created_BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_DeliveryAddressContactId",
                table: "Addresses",
                newName: "IX_Addresses_DeliveryAddressContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_CountryId",
                table: "Addresses",
                newName: "IX_Addresses_CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldOfInterestDescriptions",
                table: "FieldOfInterestDescriptions",
                columns: new[] { "FieldOfInterestId", "LanguageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldsOfInterest",
                table: "FieldsOfInterest",
                column: "FieldOfInterestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryDescriptions",
                table: "CountryDescriptions",
                columns: new[] { "CountryId", "LanguageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "BlogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Contacts_DeliveryAddressContactId",
                table: "Addresses",
                column: "DeliveryAddressContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_CountryDescriptions_Countries_CountryId",
                table: "CountryDescriptions",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldOfInterestDescriptions_FieldsOfInterest_FieldOfInterestId",
                table: "FieldOfInterestDescriptions",
                column: "FieldOfInterestId",
                principalTable: "FieldsOfInterest",
                principalColumn: "FieldOfInterestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Blogs_BlogId",
                table: "Posts",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
