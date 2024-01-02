using Microsoft.EntityFrameworkCore;
using BlogService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogService.Infrastructure.TableConfig
{
    public class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("blog_blogs");
            builder.Property(b => b.Content).HasColumnType("longtext");
            builder.Property(b => b.Tags).HasConversion(
                t => string.Join(",",t.Select(tVal => tVal.ToString())),
                t => t.Split(",",StringSplitOptions.RemoveEmptyEntries).Select(
                    tVal => (TagClass)Enum.Parse(typeof(TagClass),tVal)
                    ).ToList()
                );//设置tag的转换规则
        }
    }
}
