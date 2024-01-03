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
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogServiceDbContext _dbContext;
        public CommentRepository(BlogServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            await _dbContext.Comments.AddAsync(comment);
        }

        /// <summary>
        /// 删除指定id的评论，以及该评论的所有子评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteCommentAsync(string commentId)
        {
            //找到该评论下的所有评论进行删除
            var comments = await this.GetCommentsWithCommentIdAsync(commentId);
            if (comments.Any())
            {
                _dbContext.RemoveRange(comments);
            }
            Comment? targetComment = await this.FindOneByIdAsync(commentId);
            if(targetComment is null)
            {
                throw new Exception($"无法进行删除,原因是找不到id为{commentId}的评论");
            }
            _dbContext.Remove(targetComment);
        }

        /// <summary>
        /// 根据id查找指定的comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<Comment?> FindOneByIdAsync(string commentId)
        {
            return await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id.ToString() == commentId);
        }

        /// <summary>
        /// 查找该博客下的所有评论
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetCommentsWithBlogIdAsync(string blogId)
        {
            Guid blogGuid = Guid.Parse(blogId);
            var comments = await _dbContext.Comments.Where(c => c.BlogId == blogGuid).ToListAsync();
            return comments;
        }

        /// <summary>
        /// 查找该评论下的所有评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetCommentsWithCommentIdAsync(string commentId)
        {
            Guid guid = Guid.Parse(commentId);
            var comments = await _dbContext.Comments.Where(c => c.ParentId == guid).ToListAsync();
            return comments;
        }
    }
}
