using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public interface IBlogRepository
    {
        Task<Blog?> FindOneByIdAsync(string id);
        Task CreateBlogAsync(Blog blog);
        Task UpdateBlogAsync(Blog blog);
        Task DeleteBlogAsync(string id);
    }
}
