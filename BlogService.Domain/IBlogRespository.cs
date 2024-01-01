using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public interface IBlogRespository
    {
        Task AddNewBlog(Blog blog);
        Task UpdateBlog(string blogId,Blog blog);
        Task DeleteBlog(string blogId);

    }
}
