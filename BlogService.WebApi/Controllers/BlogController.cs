using ApiJsonResult;
using BlogDomainCommons;
using BlogRabbitHelper;
using BlogService.Domain;
using BlogService.Domain.Dtos;
using BlogService.Domain.Entities;
using BlogService.Infrastructure;
using BlogService.WebApi.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace BlogService.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlogController : ApiBaseController
    {
        private readonly BlogDomainService _blogService;
        private readonly IEventBus _eventBus;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly FileAPi.FileAPiClient _fileApiClient;
        private readonly UserApi.UserApiClient _userApiClient;

        public BlogController(BlogDomainService blogDomainService,
            IEventBus eventBus, IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            FileAPi.FileAPiClient fileApiClient,
            UserApi.UserApiClient userApiClient)
        {
            _blogService = blogDomainService;
            _eventBus = eventBus;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _fileApiClient = fileApiClient;
            _userApiClient = userApiClient;
        }

        //创建Blog
        [HttpPost]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<ResponseJsonResult<Blog>> CreateBlog([FromBody] BlogRequest blog)
        {
            //获得userId
            var token = GetToken();
            var userId = _blogService.GetCurrentUserId(token);
            Blog newBlog = new Blog(blog.Title, blog.Content, userId, blog.Tags);

            //如果含有图片，将base64转为url
            //匹配src=""
            var regex = new Regex("src\\s*=\\s*['\"].*?['\"]");
            MatchCollection match = regex.Matches(blog.Content);
            //如果包含图片
            if (match.Count > 0)
            {
                List<string> imageStrs = new List<string>();
                foreach (Match item in match)
                {
                    string[] strs = item.Value.Split("base64,");
                    imageStrs.Add("data:image/png;base64," + strs[1].Replace("'","").Replace("\"",""));
                }
                //上传图片
                var request = new FileBase64BulkRequest { 
                    Route = "/mydata/PlatformResource/", 
                    UserId = userId.ToString(), 
                    BloId = newBlog.Id.ToString() 
                };
                request.ImgStrs.AddRange(imageStrs);
                IEnumerable<string> imageUrls = await ConvertBulkBase64ToUrl(request);//图片url集合
                List<string> urlList = imageUrls.ToList();
                string blogContent = blog.Content;
                for (var i = 0; i < match.Count; i++)
                {
                    blogContent = blogContent.Replace(imageStrs[i], urlList[i]);
                    //设置预览图
                    if (i == 0) { newBlog.UpdatePreviewPhoto(urlList[0]); }
                }
                newBlog.ChangeBlog(blog.Title, blogContent, blog.Tags);
            }
            //上传blog
            var uploadBlog = await _blogService.CreateBlog(newBlog);
            //发布集成事件
            _eventBus.Publish("Blog.Create", new
            {
                id = newBlog.Id,
                blogTitle = newBlog.Title,
                blogContent = newBlog.Content,
                userId = newBlog.UserId,
                previewPhoto = newBlog.PreviewPhoto
            });
            return newBlog;
        }

        /// <summary>
        /// 删除指定的Blog
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获得个人所有博客
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<IEnumerable<Blog>> GetAllPersonalBlogs(string userId)
        {
            string token = GetToken();
            return await _blogService.FindAllPersonalBlogsAsync(userId,token) ;
        }

        /// <summary>
        /// 更新博客
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        [HttpPut]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<ResponseJsonResult<Blog>> UpdateBlog([FromBody]Blog blog)
        {
            var result = await _blogService.UpdateBlog(blog);
            if (result.StatusCode == MyStatusCode.Success)
            {
                _eventBus.Publish("Blog.Update", new { guid = blog.Id, title = blog.Title, content = blog.Content });
            }
            return result;
        }

        //获取指定博客
        [HttpGet]
        [AllowAnonymous]
        public async Task<Blog> GetUniqueBlog(string blogId)
        {
            Blog? response =  await _blogService.GetBlogById(blogId);
            if(response == null)
            {
                throw new Exception("没有该博客");
            }
            return response;
        }

        /// <summary>
        /// 根据点赞数分页返回博客
        /// </summary>
        /// <param name="index">页数(>0)</param>
        /// <param name="pageSize">每页</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<BlogWithUserInfo>> GetRecommendBlog(int index,int pageSize)
        {
            var blogs =  await _blogService.GetRecommendBlogsAsync(index,pageSize);
            return await GetBlogOwnerAvatar(blogs);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IEnumerable<string>> Test(FileBase64Request request)
        {
            FileBase64BulkRequest files = new FileBase64BulkRequest
            {
                BloId = request.BlogId,
                UserId = request.UserId,
                Route = request.FileUrl
            };
            files.ImgStrs.Add(request.Base64Str);
            var result = await ConvertBulkBase64ToUrl(files);
            return result;
        }


        [NoWrap]
        private string GetToken()
        {
            string token = this.HttpContext.Request.Headers["Authorization"]!;
            return token;
        }

        /// <summary>
        /// 将base64图片上传服务器，并返回图片路径
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        [NoWrap]
        private async Task<string> ConvertBase64ToUrl(string base64,string? userId=null,string? blogId=null)
        {
            FileBase64Request request = new FileBase64Request {
                Base64Str = base64,
                UserId = userId,
                BlogId = blogId,
                FileName = NewId.NextGuid().ToString(),
                FileUrl = "/mydata/PlatformResource/",
            };
            FileResponse response = await _fileApiClient.UploadPhotoAsync(request);
            return response.FileUrl;
        }

        /// <summary>
        /// 批量上传base64图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [NoWrap]
        private async Task<IEnumerable<string>> ConvertBulkBase64ToUrl(FileBase64BulkRequest request)
        {
            var bulkFileResponse = await _fileApiClient.BulkUpdatePhotoAsync(request);
            List<string> imageUrls = bulkFileResponse.FileUrls.Select(fileUrl => fileUrl.FileUrl).ToList();
            return imageUrls;
        }

        /// <summary>
        /// 将博客的User信息补充完整
        /// </summary>
        /// <param name="blogs"></param>
        /// <returns></returns>
        [NoWrap]
        private async Task<IEnumerable<BlogWithUserInfo>> GetBlogOwnerAvatar(IEnumerable<Blog> blogs)
        {
            //没有博客直接返回
            if (blogs.Count() == 0) throw new Exception("请求中没有任何博客");
            string? userServer = _configuration.GetSection("UserServer").Value;
            //配置文件无服务器信息直接返回
            if (userServer == null) { throw new Exception("无用户服务器信息"); }
            //存放用户信息
            List<string> userIdList = new List<string>();
            string token = GetToken();//token
            //获得所有的用户id
            foreach (var blog in blogs)
            {
                //if (blog.UserId == null) continue;
                string userId = blog.UserId.ToString();
                if (!userIdList.Contains(userId))
                {
                    userIdList.Add(userId);
                }
            };
            if (userIdList.Count <= 0) { throw new Exception("博客无用户信息"); }

            var protoIdList = userIdList.Select(id => new UserId { Id = id }).ToList();
            BulkUserId ids = new BulkUserId();
            ids.Ids.AddRange(userIdList);
            BulkUserInfo userInfos  = await _userApiClient.GetBulkUserInfoAsync(ids);
            //如果无数据直接返回
            if (userInfos == null || userInfos.UserInfos.Count == 0) { throw new Exception("未查询到用户信息"); }
            List<BlogWithUserInfo> result =  blogs.Select(blog =>
            {
                string userId = blog.UserId.ToString();
                Protos.UserInfo userInfo = userInfos.UserInfos.Where(info => info.Id == userId).First();
                return new BlogWithUserInfo(blog.Id.ToString(),blog.Title,blog.Content,userId,userInfo.UserName,userInfo.AvatarUrl,blog.PreviewPhoto!,blog.Tags);
            }).ToList();
            return result;
        }
    }
}
