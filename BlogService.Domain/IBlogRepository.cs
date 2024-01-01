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
        Task<Blog> FindOneById(string id);
        Task CreateBlog(Blog blog);
        Task UpdateBlog(Blog blog);
        Task Task<Blog>(string id);
    }
}
