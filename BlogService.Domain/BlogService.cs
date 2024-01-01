using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public class BlogService
    {
        private readonly IBlogRepository blogRepository;
        public BlogService(IBlogRepository blogRepository) 
        {
            this.blogRepository = blogRepository;
        }
        public async Task ChangeBlogContent(Blog blog)
        {
            await this.blogRepository.UpdateBlogAsync(blog);
        }
    }
}
