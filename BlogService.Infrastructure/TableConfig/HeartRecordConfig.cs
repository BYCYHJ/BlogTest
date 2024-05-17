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
    public class HeartRecordConfig : IEntityTypeConfiguration<HeartRecord>
    {
        public void Configure(EntityTypeBuilder<HeartRecord> builder)
        {
            builder.ToTable("blog_heart_record");
            builder.Property(h => h.Type)
                .HasConversion(
                type => type.ToString(),
                s => (HeartType)Enum.Parse(typeof(HeartRecord), s));
            builder.HasNoKey();
        }
    }
}
