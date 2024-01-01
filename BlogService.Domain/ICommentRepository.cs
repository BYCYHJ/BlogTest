using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public interface ICommentRepository
    {
        Task AddBlogComment(Guid blogId,string content);
        Task AddReplyComment(Guid commentId,string content);
        Task DeleteComment(Guid commentId);
    }
}
