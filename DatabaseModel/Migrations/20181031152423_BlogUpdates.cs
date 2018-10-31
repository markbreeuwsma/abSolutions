using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class BlogUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Blogs",
                type: "nvarchar",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Blogs",
                type: "DateTime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Blogs",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs",
                columns: new[] { "Created", "BlogId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Blogs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Blogs",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DateTime");
        }
    }
}
