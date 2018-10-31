using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class BlogUpdates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs",
                columns: new[] { "Created", "BlogId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_Created_BlogId",
                table: "Blogs",
                columns: new[] { "Created", "BlogId" });
        }
    }
}
