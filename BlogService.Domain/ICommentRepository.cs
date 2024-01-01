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
        Task<IEnumerable<Comment>> GetCommentsWithBlogId(string blogId);
        Task CreateComment(Comment comment);
        Task DeleteComment(string commentId);
    }
}
