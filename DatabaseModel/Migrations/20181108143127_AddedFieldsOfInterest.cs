using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class AddedFieldsOfInterest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldsOfInterest",
                columns: table => new
                {
                    FieldOfInterestId = table.Column<string>(maxLength: 5, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsOfInterest", x => x.FieldOfInterestId);
                });

            migrationBuilder.CreateTable(
                name: "FieldOfInterestDescriptions",
                columns: table => new
                {
                    LanguageId = table.Column<string>(maxLength: 3, nullable: false),
                    Description = table.Column<string>(maxLength: 80, nullable: false),
                    FieldOfInterestId = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOfInterestDescriptions", x => new { x.FieldOfInterestId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_FieldOfInterestDescriptions_FieldsOfInterest_FieldOfInterestId",
                        column: x => x.FieldOfInterestId,
                        principalTable: "FieldsOfInterest",
                        principalColumn: "FieldOfInterestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldOfInterestDescriptions_LanguageId_FieldOfInterestId",
                table: "FieldOfInterestDescriptions",
                columns: new[] { "LanguageId", "FieldOfInterestId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldOfInterestDescriptions");

            migrationBuilder.DropTable(
                name: "FieldsOfInterest");
        }
    }
}
