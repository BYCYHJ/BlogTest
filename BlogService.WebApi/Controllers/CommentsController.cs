using ApiJsonResult;
using BlogDomainCommons;
using BlogJWT;
using BlogService.Domain;
using BlogService.Domain.Entities;
using BlogService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;

namespace BlogService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpPost]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task CreateComment([FromBody]CommentResponse commentRes)
        {
            string userId = null;
            if( String.IsNullOrEmpty(commentRes.userId))
            {
                userId = GetCurrentUserId();
            }
            else
            {
                userId = commentRes.userId;
            }
            Comment comment = new Comment(content:commentRes.content,blogId:commentRes.blogId,userId:userId,parentId:commentRes.parentId);
            await _commentRepository.CreateCommentAsync(comment);
        }

        [HttpDelete]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<ResponseJsonResult<Comment>> DeletCommentById(string commentId)
        {
            return await _commentRepository.DeleteCommentAsync(commentId);
        }

        [HttpGet]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<IEnumerable<Comment>> GetBlogAllComments(string blogId,int index,int pageSize)
        {
            return await _commentRepository.GetBlogCommentsWithPagesAsync(blogId,pageSize:pageSize,index:index);
        }

        [HttpGet]
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task<IEnumerable<Comment>> GetChildrenComments(string commentId, int index, int pageSize)
        {
            return await _commentRepository.GetChildrenCommentsWithPagesAsync(commentId:commentId, pageSize: pageSize, index: index);
        }

        //获取当前用户id
        [NoWrap]
        private string GetCurrentUserId()
        {
            var token = this.HttpContext.Request.Headers["Authorization"];
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
    }
}
