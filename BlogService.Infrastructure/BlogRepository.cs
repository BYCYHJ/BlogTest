using ApiJsonResult;
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

        /// <summary>
        /// 创建一个博客
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        public async Task CreateBlogAsync(Blog blog)
        {
            await _dbContext.Blogs.AddAsync(blog);
        }

        /// <summary>
        /// 根据id删除一个博客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据id查找博客，并包含评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Blog?> FindOneByIdWithCommentsAsync(string id)
        {
            Guid guid = Guid.Parse(id);
            Blog? blog =  await _dbContext.Blogs.Where(b => b.Id == guid).
                Include(b => b.Comments).FirstOrDefaultAsync();
            return blog;
        }

        /// <summary>
        /// 根据id查找一个博客，无评论内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Blog?> FindOneByIdNoCommentsAsync(string id)
        {
            Guid guid = Guid.Parse(id);
            return await _dbContext.Blogs.Where(b => b.Id == guid).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 更新个人博客
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获得所有个人博客
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Blog>> GetPersonalAllBlogsAsync(string userId)
        {
            Guid userGuid = Guid.Parse(userId);
            return await _dbContext.Blogs.Where(b => b.UserId == userGuid).ToListAsync();
        }

        /// <summary>
        /// 根据点赞数排序获得博客
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Blog>> GetRecommendBlogsAsync(int index, int pageSize)
        {
            return await _dbContext.Blogs
                .OrderBy(b => b.StartCount)
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> BlogExistAsync(Guid id)
        {
            return await _dbContext.Blogs.AnyAsync(b => b.Id == id);
        }
    }
}
