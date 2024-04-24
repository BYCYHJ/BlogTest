using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Infrastructure.TableConfig
{
    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("blog_comment");
            builder.Property(c => c.Content).HasMaxLength(600);
            builder.HasOne<Blog>(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade);//Blog一对多Comment,启用级联删除
            builder.HasOne<Comment>(c => c.ParentComment)
                .WithMany(c => c.ChildrenComments)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);//自引用一对多，启用级联删除
            builder.HasIndex(c => new { c.UserId, c.BlogId, c.ParentId,c.HighestCommentId});
        }
    }
}
