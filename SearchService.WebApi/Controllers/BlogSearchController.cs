using ApiJsonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SearchService.Domain;

namespace SearchService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlogSearchController : ApiBaseController
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public BlogSearchController(ISearchRepository searchRepository, IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            _searchRepository = searchRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ResponseJsonResult<SearchBlogResponse>?> GetBlogsWithKeyword(string keyword,int index,int pageSize)
        {
            SearchBlogResponse blogs =  await _searchRepository.SearchBlogAsync(keyword, index, pageSize);
            blogs.Blogs =  await GetBlogOwnerAvatar(blogs.Blogs);
            return blogs;
        }

        /// <summary>
        /// 将博客的User信息补充完整
        /// </summary>
        /// <param name="blogs"></param>
        /// <returns></returns>
        [NoWrap]
        private async Task<IEnumerable<BlogRecord>> GetBlogOwnerAvatar(IEnumerable<BlogRecord> blogs)
        {
            //没有博客直接返回
            if (blogs.Count() == 0) return blogs;
            string? userServer = _configuration.GetSection("UserServer").Value;
            //配置文件无服务器信息直接返回
            if (userServer == null) { return blogs; }
            //存放用户信息
            List<string> userIdList = new List<string>();
            string token = GetToken();//token
            //获得所有的用户id
            foreach (var blog in blogs) 
            {
                if (blog.UserId == null) continue;
                if (!userIdList.Contains(blog.UserId))
                {
                    userIdList.Add(blog.UserId);
                }
            };
            if (userIdList.Count <= 0) { return blogs; }
            //请求获取用户信息
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(userServer);
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization,"Bearer " + token);
            var response = await client.PostAsJsonAsync("/api/User/GetBulkUserInfo", userIdList);
            if (!response.IsSuccessStatusCode)
            {
                return blogs;
            }
            string content = await response.Content.ReadAsStringAsync();
            var userInfoList = JsonConvert.DeserializeObject<IEnumerable<UserInfo>>(content);
            //如果无数据直接返回
            if(userInfoList == null || userInfoList.Count() == 0) { return blogs; }
            //赋值blog信息
            foreach(var blog in blogs)
            {
                if(blog.UserId == null) { continue; }
                var targetUser = userInfoList.Where(u => u.Id == blog.UserId).First();
                blog.AvatarUrl = targetUser.AvatarUrl;
                blog.UserName = targetUser.userName;
            }
            return blogs;
        }

        [NoWrap]
        private string GetToken()
        {
            string token = this.HttpContext.Request.Headers["Authorization"]!;
            return token;
        }
    }
}
