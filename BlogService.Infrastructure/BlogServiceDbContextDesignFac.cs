using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Infrastructure
{
    public class BlogServiceDbContextDesignFac : IDesignTimeDbContextFactory<BlogServiceDbContext>
    {
        public BlogServiceDbContext CreateDbContext(string[] args)
        {
            string conStr = "Server=132.232.108.176;Port=3306;Database=blog_d1;Uid=root;Pwd=15550239";
            var builder = new DbContextOptionsBuilder<BlogServiceDbContext>();
            builder.UseMySql(conStr,ServerVersion.Parse("8.0.34-mysql"));
            BlogServiceDbContext dbContext = new BlogServiceDbContext(builder.Options);
            return dbContext;   
        }
    }
}
