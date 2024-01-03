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
        Task<IEnumerable<Comment>> GetCommentsWithBlogIdAsync(string blogId);
        Task<IEnumerable<Comment>> GetCommentsWithCommentIdAsync(string commentId);
        Task<Comment?> FindOneByIdAsync(string commentId);
        Task CreateCommentAsync(Comment comment);
        Task DeleteCommentAsync(string commentId);
    }
}
