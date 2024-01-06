using Microsoft.EntityFrameworkCore;
using BlogService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CommenHelper;

namespace BlogService.Infrastructure.TableConfig
{
    public class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("blog_blogs");
            builder.Property(b => b.Content).HasColumnType("longtext");
            builder.Property(b => b.Tags).HasConversion(
                t => string.Join(",", t.Select(tVal => tVal.ToString())),
                t => t.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(
                    tVal => (TagClass)Enum.Parse(typeof(TagClass), tVal)
                    ).ToList(),
                valueComparer: new ValueComparer<List<TagClass>>(
                    //比较IEnurable<T>的元素是否相等
                    (t1, t2) => ListHelper.CompareList<TagClass>(t1, t2),
                    //将List的每个元素获取hash，并将hash相加得到List的hash
                    t => t.Sum(x => x.GetHashCode())
                    )
                );
        }
    }
}
