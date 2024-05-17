using ApiJsonResult;
using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public interface ICommentRepository
    {
        Task<Comment?> FindOneByIdAsync(string commentId);
        Task<IEnumerable<Comment>> GetBlogCommentsWithPagesAsync(string blogId,int pageSize,int index);
        Task<IEnumerable<Comment>> GetChildrenCommentsWithPagesAsync(string commentId,int pageSize,int index);
        Task<IEnumerable<Comment>> GetBlogCommentsAsync(string blogId);
        Task<IEnumerable<Comment>> GetChildrenCommentsAsync(string commentId);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<ResponseJsonResult<Comment>> DeleteCommentAsync(string commentId);
        /// <summary>
        /// 查找该条评论是否存在
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<bool> CommentExistAsync(Guid guid);

        Task<(Comment, int heartCount)> GetCommentByIdAsync(string commentId);
    }
}
