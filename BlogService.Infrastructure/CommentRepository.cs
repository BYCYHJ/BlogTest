using ApiJsonResult;
using BlogService.Domain;
using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public async Task<bool> CommentExistAsync(Guid guid)
        {
            return await _dbContext.Comments.AnyAsync(c => c.Id ==  guid);
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _dbContext.Comments.AddAsync(comment);
            return comment;
        }

        /// <summary>
        /// 删除指定id的评论，以及该评论的所有子评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseJsonResult<Comment>> DeleteCommentAsync(string commentId)
        {
            //找到该评论下的所有评论进行删除
            var comments = await GetChildrenCommentsAsync(commentId);
            if (comments.Any())
            {
                _dbContext.RemoveRange(comments);
            }
            Comment? targetComment = await this.FindOneByIdAsync(commentId);
            if(targetComment is null)
            {
                return ErrorResult($"无法进行删除,原因是找不到id为{commentId}的评论");
            }
            _dbContext.Remove(targetComment);
            return ResponseJsonResult<Comment>.Succeeded;
        }

        /// <summary>
        /// 根据id查找指定评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<Comment?> FindOneByIdAsync(string commentId)
        {
            return await _dbContext.Comments.Where(c => c.Id.ToString() == commentId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取博客下的所有评论，不分页
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetBlogCommentsAsync(string blogId)
        {
            Guid blogGuid = Guid.Parse(blogId);
            return await _dbContext.Comments.Where(c => c.BlogId == blogGuid).ToListAsync();
        }

        /// <summary>
        /// 分页查询：查找该博客下的所有一级评论
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="pageSize"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<Comment>> GetBlogCommentsWithPagesAsync(string blogId, int pageSize, int index)
        {
            //查找为该博客的评论，且没有父评论的评论
            var comments = await _dbContext.Comments
                .Where(c => c.BlogId.ToString() == blogId && (c.ParentId == null || c.ParentId == Guid.Empty))
                .OrderBy(c => c.CreateOnTime)
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return comments;
        }

        /// <summary>
        /// 获取所有子评论，不分页
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetChildrenCommentsAsync(string commentId)
        {
            Guid parentId = Guid.Parse(commentId);
            return await _dbContext.Comments.Where(c => c.ParentId == parentId).ToListAsync();
        }

        /// <summary>
        /// 分页查询：查找评论下的所有子评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="pageSize"></param>
        /// <param name="index">从1开始</param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetChildrenCommentsWithPagesAsync(string commentId, int pageSize, int index)
        {
            var comments = await _dbContext.Comments
                .Where(c => c.HighestCommentId == commentId)
                .Skip(pageSize * (index - 1))
                .Take(pageSize)
                .Include(commnet => commnet.ParentComment)
                .ToListAsync();
            return comments;
        }

        /// <summary>
        /// 根据id寻找博客记录，并带回点赞和收藏数量
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<(Comment, int heartCount)> GetCommentByIdAsync(string commentId)
        {
            var result = await _dbContext.Comments
                .Where(c => c.Id.ToString() == commentId)
                .GroupJoin(
                _dbContext.Hearts,
                comment => comment.Id.ToString(),
                heart => heart.ObjectId,
                (comment, heart) => new
                {
                    Comment = comment,
                    ActualCount = heart.Count()
                }
                ).FirstAsync();
            if (result == null) { throw new Exception($"没有找到id为{commentId}的博客记录"); }
            return (result.Comment, result.ActualCount);
        }

        //快捷错误result
        private ResponseJsonResult<Comment> ErrorResult(string errorMsg)
        {
            var result = ResponseJsonResult<Comment>.Failed;
            result.Message = errorMsg;
            return result;
        }

    }
}
