using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Config
{
    public class ServerFileConfig : IEntityTypeConfiguration<ServerFile>
    {
        public void Configure(EntityTypeBuilder<ServerFile> builder)
        {
            builder.ToTable("blog_files");

        }
    }
}
