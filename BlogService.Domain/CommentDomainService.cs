﻿using ApiJsonResult;
using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public class CommentDomainService
    {
        private readonly IBlogRepository blogRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IHeartRecordRepository heartRecordRepository;

        public CommentDomainService(IBlogRepository blogRepository, ICommentRepository commentRepository, IHeartRecordRepository heartRecordRepository)
        {
            this.blogRepository = blogRepository;
            this.commentRepository = commentRepository;
            this.heartRecordRepository = heartRecordRepository;
        }

        /// <summary>
        /// 创建评论，如果是子评论会检查父评论是否存在
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            bool parentExist = true;
            bool highestExist = true;
            bool blogExist = true;
            blogExist = await blogRepository.BlogExistAsync(comment.BlogId);
            if(!blogExist) { throw new Exception($"创建评论失败，因为id为{comment.BlogId.ToString()}的博客不存在"); }
            if (comment.ParentId != null)
            {
                Guid guid = (Guid)comment.ParentId;
                //如果有父级和顶级评论，查看是否存在
                parentExist = await commentRepository.CommentExistAsync(guid);
            }
            if(comment.HighestCommentId != null && comment.HighestCommentId != string.Empty) {
                Guid guid = Guid.Parse(comment.HighestCommentId);
                highestExist = await commentRepository.CommentExistAsync(guid);
            }
            if(!parentExist || !highestExist)
            {
                throw new Exception("所属评论不存在，无法作为子评论创建");
            }
            return await commentRepository.CreateCommentAsync(comment);
        }
        
        /// <summary>
        /// 获取第一层评论
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetHighestCommentsAsync(string blogId,int page,int size)
        {
            return await commentRepository.GetBlogCommentsWithPagesAsync(blogId, index:page,pageSize:size) ;
        }

        /// <summary>
        /// 获取评论的子评论
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Comment>> GetChildrenCommentsAsync(string parentId, int page, int size)
        {
            return await commentRepository.GetChildrenCommentsWithPagesAsync(parentId,index:page,pageSize: size);
        }

        /// <summary>
        /// 删除一条评论，及其所有子评论（级联删除）
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<ResponseJsonResult<Comment>> DeleteCommentAsync(string commentId)
        {
            return await commentRepository.DeleteCommentAsync(commentId);
        }

        /// <summary>
        /// 根据id返回评论，附带真实点赞数和收藏数
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<(Comment, int heartCount)> GetCommentByIdAsync(string commentId)
        {
            return await commentRepository.GetCommentByIdAsync(commentId);
        }

        /// <summary>
        /// 创建一条 用户-评论-喜欢 记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<HeartRecord> CreateLoveCommentRecordAsync(string userId, string commentId)
        {
            return await heartRecordRepository.GetHeartRecord(userId, commentId, HeartType.Comment);
        }
    }
}
