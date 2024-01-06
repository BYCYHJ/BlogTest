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
    public class BlogController : ControllerBase
    {
        private readonly BlogDomainService _blogService;
        public BlogController(BlogDomainService blogDomainService) { 
            _blogService = blogDomainService;
        }

        [HttpPost]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task CreateBlog(string title,string content,List<TagClass> tags,string? userId = null)
        {
            var token = GetToken();
            await _blogService.CreateBlog(title, content, tags, token, userId);
        }

        private string GetToken()
        {
            string token = this.HttpContext.Request.Headers["Authorization"]!;
            return token;
        }
    }
}
