using ApiJsonResult;
using BlogDomainCommons;
using BlogJWT;
using BlogService.Domain;
using BlogService.Domain.Dtos;
using BlogService.Domain.Entities;
using BlogService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BlogService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentDomainService _commentDomainService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public CommentsController(CommentDomainService commentDomainService, IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            _commentDomainService = commentDomainService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task CreateComment([FromBody]CommentRequest commentReq)
        {
            string? userId = null;
            if( String.IsNullOrEmpty(commentReq.userId))
            {
                userId = GetCurrentUserId();
            }
            else
            {
                userId = commentReq.userId;
            }
            Comment comment = new Comment(content:commentReq.content,blogId:commentReq.blogId,userId:userId,parentId:commentReq.parentId,highestId:commentReq.highestCommentId);
            await _commentDomainService.CreateCommentAsync(comment);
        }

        [HttpDelete]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<ResponseJsonResult<Comment>> DeletCommentById(string commentId)
        {
            return await _commentDomainService.DeleteCommentAsync(commentId);
        }

        [HttpGet]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<IEnumerable<CommentResponse>> GetBlogHighestComments(string blogId, int index, int pageSize)
        {
            var comments = await _commentDomainService.GetHighestCommentsAsync(blogId, size: pageSize, page: index);
            return await AddUserInfoToComments(comments);
        }

        [HttpGet]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<IEnumerable<CommentResponse>> GetChildrenComments(string commentId, int index, int pageSize)
        {
            var comments =  await _commentDomainService.GetChildrenCommentsAsync(parentId:commentId, size: pageSize, page: index);
            return await AddUserInfoToComments(comments);
        }

        //获取当前用户id
        [NoWrap]
        private string GetCurrentUserId()
        {
            var token = GetToken();
            var payloadClaims = TokenCommen.GetPayloadInfo(token!);
            List<Claim> claims = new List<Claim>();
            if (payloadClaims != null && payloadClaims.Any())
            {
                claims = payloadClaims.ToList();
            }
            string? userId = null;
            foreach (var claim in claims)
            {
                if (claim != null && claim.Type == ClaimTypes.NameIdentifier)
                {
                    userId = claim.Value;
                }
            }
            if (userId is null)
            {
                throw new Exception("无法取到用户id");
            }
            return userId;
        }

        //获取请求中的token
        [NoWrap]
        private string GetToken()
        {
            string token = this.HttpContext.Request.Headers["Authorization"]!;
            return token;
        }

        /// <summary>
        /// 批量获取用户信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns>包含了用户信息的集合</returns>
        [NoWrap]
        private async Task<List<UserInfo>> GetBulkUserInfos(IEnumerable<string> userIds)
        {
            if (!userIds.Any()) { throw new Exception("无用户信息"); }
            string? userServer = _configuration.GetSection("UserServer").Value;
            //配置文件无服务器信息直接返回
            if (userServer == null) { throw new Exception("无用户服务器信息"); }
            //发送请求
            var client =  _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(userServer);
            string token = GetToken();//token
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer " + token);
            var response = await client.PostAsJsonAsync("/api/User/GetBulkUserInfo", userIds);
            //如果非成功状态，抛出异常
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("查询用户请求未成功");
            }
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<UserInfo>>(content).ToList();
        }

        [NoWrap]
        private async Task<List<CommentResponse>> AddUserInfoToComments(IEnumerable<Comment> comments)
        {
            List<string> userIds = new List<string>();
            //拿取博客中所包含的所有用户id
            foreach (Comment comment in comments)
            {
                string userId = comment.UserId.ToString();
                if (!userIds.Contains(userId)) { userIds.Add(userId); }
                if (comment.ParentComment != null)
                {
                    string replyId = comment.ParentComment.UserId.ToString();
                    if (!userIds.Contains(replyId))
                    {
                        userIds.Add(replyId);
                    }
                }
            }
            List<UserInfo> userInfos = await GetBulkUserInfos(userIds);
            List<CommentResponse> result = comments.Select(comment =>
            {
                UserInfo user = userInfos.Where(info => info.id == comment.UserId.ToString()).First();
                string? replyName = null;
                if (comment.ParentComment is not null)
                {
                    replyName = userInfos.Where(info => info.id == comment.ParentComment!.UserId.ToString()).Select(i => i.userName).First();
                }
                CommentResponse response = new CommentResponse(
                    id:comment.Id.ToString(),
                    content: comment.Content,
                    parentId: comment.ParentId.ToString(),
                    publishDate: comment.CreateOnTime.ConvertToChineseTime(),
                    userId: user.id,
                    userName: user.userName,
                    avatarUrl: user.avatarUrl,
                    starCount:comment.StarCount,
                    replyUserName:replyName
                    );
                return response;
            }).ToList();
            return result;
        }
    }
}
