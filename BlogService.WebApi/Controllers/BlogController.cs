using ApiJsonResult;
using BlogDomainCommons;
using BlogService.Domain;
using BlogService.Domain.Entities;
using BlogService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogService.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlogController : ApiBaseController
    {
        private readonly BlogDomainService _blogService;
        private readonly IBlogRepository _blogRepository;

        public BlogController(BlogDomainService blogDomainService,IBlogRepository blogRepository) { 
            _blogService = blogDomainService;
            _blogRepository = blogRepository;
        }

        //创建Blog
        [HttpPost]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task CreateBlog(string title,string content,List<TagClass> tags,string? userId = null)
        {
            var token = GetToken();
            await _blogService.CreateBlog(title, content, tags, token, userId);
        }

        //删除指定的Blog
        [HttpDelete]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]

        public async Task<ResponseJsonResult<Blog>> DeleteBlog(string blogId)
        {
            return await _blogService.DeleteBlogAsync(blogId);
        }

        //获得个人所有博客
        [HttpGet]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<IEnumerable<Blog>> GetAllPersonalBlogs(string userId)
        {
            string token = GetToken();
            return await _blogService.FindAllPersonalBlogsAsync(userId,token) ;
        }

        //更新博客
        [HttpPut]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<ResponseJsonResult<Blog>> UpdateBlog(Blog blog)
        {
            return await _blogRepository.UpdateBlogAsync(blog);
        }


        [NoWrap]

        private string GetToken()
        {
            string token = this.HttpContext.Request.Headers["Authorization"]!;
            return token;
        }
    }
}
