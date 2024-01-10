using ApiJsonResult;
using BlogDomainCommons;
using BlogRabbitHelper;
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
        private readonly IEventBus _eventBus;

        public BlogController(BlogDomainService blogDomainService,IBlogRepository blogRepository,IEventBus eventBus) { 
            _blogService = blogDomainService;
            _blogRepository = blogRepository;
            _eventBus = eventBus;
        }

        //创建Blog
        [HttpPost]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task CreateBlog(string title,string content,List<TagClass> tags,string? userId = null)
        {
            var token = GetToken();
            Blog newBlog = await _blogService.CreateBlog(title, content, tags, token, userId);
            //发布集成事件
            _eventBus.Publish("Blog.Create",new {guid=newBlog.Id,title=newBlog.Title,content=newBlog.Content});
        }

        //删除指定的Blog
        [HttpDelete]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]

        public async Task<ResponseJsonResult<Blog>> DeleteBlog(string blogId)
        {
            var result = await _blogService.DeleteBlogAsync(blogId);
            if(result.StatusCode == MyStatusCode.Success)
            {
                _eventBus.Publish("Blog.Delete",Guid.Parse(blogId));
            }
            return result;
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
            var result = await _blogRepository.UpdateBlogAsync(blog);
            if (result.StatusCode == MyStatusCode.Success)
            {
                _eventBus.Publish("Blog.Update", new { guid = blog.Id, title = blog.Title, content = blog.Content });
            }
            return result;
        }


        [NoWrap]

        private string GetToken()
        {
            string token = this.HttpContext.Request.Headers["Authorization"]!;
            return token;
        }
    }
}
