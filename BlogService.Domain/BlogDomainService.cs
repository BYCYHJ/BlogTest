using BlogJWT;
using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public class BlogDomainService
    {
        private readonly IBlogRepository blogRepository;
        private readonly ICommentRepository commentRepository;
        //private readonly TokenCommen tokenCommon;

        public BlogDomainService(IBlogRepository blogRepository,ICommentRepository commentRepository) 
        {
            this.blogRepository = blogRepository;
            this.commentRepository = commentRepository;
            //this.tokenCommon = new TokenCommon();
        }

        /// <summary>
        /// 创建博客,若没有传userId则根据token解析
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="tags"></param>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task CreateBlog(string title,string content,List<TagClass>? tags, string token,string? userId = null)
        {
            Guid userid;
            if (userId != null)
            {
                userid = Guid.Parse(userId);
            }
            else {
                userid = GetCurrentUserId(token);
            }
            Blog blog = new Blog(title,content,userid,tags);
            await blogRepository.CreateBlogAsync(blog);
        }

        /// <summary>
        /// 根据id查找博客，返回博客以及包含的评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<(Blog targetBlog,List<Comment> comments)> GetTargetBlogById(string id)
        {
            Blog? targetBlog = await blogRepository.FindOneByIdAsync(id);
            if (targetBlog == null)
            {
                throw new Exception($"找不到id为{id}的博客");
            }
            var comments = new List<Comment>();
            var blogComments = await commentRepository.GetCommentsWithBlogId(id);
            if(blogComments != null && blogComments.Any())
            {
                comments = blogComments.ToList();
            }
            return (targetBlog,comments);
        }

        /// <summary>
        /// 根据token获取id
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Guid GetCurrentUserId(string token)
        {
            var payloadClaims = TokenCommen.GetPayloadInfo(token);
            List<Claim> claims = new List<Claim>();
            if(payloadClaims != null && payloadClaims.Any())
            {
                claims = payloadClaims.ToList();
            }
            string? userId = null;
            foreach (var claim in claims)
            {
                if(claim != null && claim.Type == ClaimTypes.NameIdentifier)
                {
                    userId = claim.Value;
                }
            }
            if (userId is null)
            {
                throw new Exception("无法取到用户id");
            }
            return Guid.Parse(userId);
        }

    }
}
