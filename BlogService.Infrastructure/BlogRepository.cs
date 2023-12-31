﻿using ApiJsonResult;
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

        public async Task<ResponseJsonResult<Blog>> DeleteBlogAsync(string id)
        {
            Blog? targetBlog = await FindOneByIdWithCommentsAsync(id);
            if(targetBlog is null)
            {
                return ErrorResult($"无法删除，原因是找不到id为{id}的博客");
            }
            _dbContext.Blogs.Remove(targetBlog);
            return ResponseJsonResult<Blog>.Succeeded;
        }

        public async Task<Blog?> FindOneByIdWithCommentsAsync(string id)
        {
            Guid guid = Guid.Parse(id);
            Blog? blog =  await _dbContext.Blogs.Where(b => b.Id == guid).
                Include(b => b.Comments).FirstOrDefaultAsync();
            return blog;
        }

        public async Task<Blog?> FindOneByIdNoCommentsAsync(string id)
        {
            Guid guid = Guid.Parse(id);
            return await _dbContext.Blogs.Where(b => b.Id == guid).FirstOrDefaultAsync();
        }


        public async Task<ResponseJsonResult<Blog>> UpdateBlogAsync(Blog blog)
        {
            string blogId = blog.Id.ToString();
            Blog? targetBlog = await FindOneByIdNoCommentsAsync(blogId);
            if(targetBlog is null)
            {
                return ErrorResult($"无法更新，原因是找不到id为{blogId}的博客");
            }
            _dbContext.Entry(targetBlog).CurrentValues.SetValues(blog);
            return ResponseJsonResult<Blog>.Succeeded;
        }

        //快捷错误result
        private ResponseJsonResult<Blog> ErrorResult(string errorMsg)
        {
            var result = ResponseJsonResult<Blog>.Failed;
            result.Message = errorMsg;
            return result;
        }

        public async Task<IEnumerable<Blog>> GetPersonalAllBlogsAsync(string userId)
        {
            Guid userGuid = Guid.Parse(userId);
            return await _dbContext.Blogs.Where(b => b.UserId == userGuid).ToListAsync();
        }
    }
}
