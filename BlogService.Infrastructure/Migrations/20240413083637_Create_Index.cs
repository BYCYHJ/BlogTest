using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_blog_comment_UserId_BlogId_ParentId",
                table: "blog_comment",
                columns: new[] { "UserId", "BlogId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_blog_blogs_UserId",
                table: "blog_blogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_blog_comment_UserId_BlogId_ParentId",
                table: "blog_comment");

            migrationBuilder.DropIndex(
                name: "IX_blog_blogs_UserId",
                table: "blog_blogs");
        }
    }
}
