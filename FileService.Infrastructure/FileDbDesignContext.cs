using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    internal class FileDbDesignContext : IDesignTimeDbContextFactory<FileDbContext>
    {
        public FileDbContext CreateDbContext(string[] args)
        {
            string conStr = "Server=132.232.108.176;Port=3306;Database=blog_d1;Uid=root;Pwd=15550239";
            var builder = new DbContextOptionsBuilder<FileDbContext>();
            builder.UseMySql(conStr, ServerVersion.Parse("8.0.34-mysql"));
            FileDbContext dbContext = new FileDbContext(builder.Options);
            return dbContext;
        }
    }
}
