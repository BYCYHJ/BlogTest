using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_heighestComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blog_comment_blog_comment_ParentId",
                table: "blog_comment");

            migrationBuilder.DropIndex(
                name: "IX_blog_comment_UserId_BlogId_ParentId",
                table: "blog_comment");

            migrationBuilder.AddColumn<string>(
                name: "HighestCommentId",
                table: "blog_comment",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_blog_comment_UserId_BlogId_ParentId_HighestCommentId",
                table: "blog_comment",
                columns: new[] { "UserId", "BlogId", "ParentId", "HighestCommentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_blog_comment_blog_comment_ParentId",
                table: "blog_comment",
                column: "ParentId",
                principalTable: "blog_comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blog_comment_blog_comment_ParentId",
                table: "blog_comment");

            migrationBuilder.DropIndex(
                name: "IX_blog_comment_UserId_BlogId_ParentId_HighestCommentId",
                table: "blog_comment");

            migrationBuilder.DropColumn(
                name: "HighestCommentId",
                table: "blog_comment");

            migrationBuilder.CreateIndex(
                name: "IX_blog_comment_UserId_BlogId_ParentId",
                table: "blog_comment",
                columns: new[] { "UserId", "BlogId", "ParentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_blog_comment_blog_comment_ParentId",
                table: "blog_comment",
                column: "ParentId",
                principalTable: "blog_comment",
                principalColumn: "Id");
        }
    }
}
