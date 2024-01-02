using BlogService.Domain;
using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Infrastructure
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogServiceDbContext _dbContext;

        public BlogRepository(BlogServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateBlogAsync(Blog blog)
        {
            await _dbContext.Blogs.AddAsync(blog);
        }

        public async Task DeleteBlogAsync(string id)
        {
            Blog? targetBlog = await FindOneByIdAsync(id);
            if(targetBlog is null)
            {
                throw new Exception($"无法删除，原因是找不到id为{id}的博客");
            }
            _dbContext.Blogs.Remove(targetBlog);
        }

        public async Task<Blog?> FindOneByIdAsync(string id)
        {
            Guid guid = Guid.Parse(id);
            Blog? blog =  await _dbContext.Blogs.Where(b => b.Id == guid).FirstOrDefaultAsync();
            return blog;
        }


        public async Task UpdateBlogAsync(Blog blog)
        {
            string blogId = blog.Id.ToString();
            Blog? targetBlog = await FindOneByIdAsync(blogId);
            if(targetBlog is null)
            {
                throw new Exception($"无法更新，原因是找不到id为{blogId}的博客");
            }
            _dbContext.Entry(targetBlog).CurrentValues.SetValues(blog);
        }
    }
}
