using ApiJsonResult;
using BlogJWT;
using BlogService.Domain.Entities;
using CommenHelper;
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
        public async Task<Blog> CreateBlog(string title,string content,List<TagClass>? tags, string token,string? userId = null,string? previewStr=null)
        {
            Guid userid;
            if (userId != null)
            {
                userid = Guid.Parse(userId);
            }
            else {
                userid = GetCurrentUserId(token);
            }
            Blog blog = new Blog(title,content,userid,tags,previewStr);
            await blogRepository.CreateBlogAsync(blog);
            return blog;
        }

        /// <summary>
        /// 创建博客
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        public async Task<Blog> CreateBlog(Blog blog)
        {
            await blogRepository.CreateBlogAsync(blog);
            return blog;
        }

        /// <summary>
        /// 更新博客
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        public async Task<ResponseJsonResult<Blog>> UpdateBlog(Blog blog)
        {
            return await blogRepository.UpdateBlogAsync(blog);
        }

        /// <summary>
        /// 根据id返回唯一博客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Blog?> GetBlogById(string id)
        {
            return await blogRepository.FindOneByIdNoCommentsAsync(id);
        }

        /// <summary>
        /// 根据赞数排序分页返回blog
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Blog>> GetRecommendBlogsAsync(int index,int pageSize)
        {
            var blogs = await blogRepository.GetRecommendBlogsAsync(index, pageSize);
            //将blog的文本内容替换为无html标签
            foreach (var blog in blogs)
            {
                blog.ChangeBlog(HtmlHelper.RemoveTags(blog.Content));
            }
            return blogs;
        }

        /// <summary>
        /// 删除博客，以及博客下的所有评论
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<ResponseJsonResult<Blog>> DeleteBlogAsync(string blogId)
        {
            Blog? blog = await blogRepository.FindOneByIdWithCommentsAsync(blogId);
            if (blog == null)
            {
                return ErrorResult($"无法删除，原因是找不到id为{blogId}的博客");
            }
            var comments = blog.Comments;
            foreach (var comment in comments)
            {
                var result = await commentRepository.DeleteCommentAsync(comment.Id.ToString());
                if(result.StatusCode != MyStatusCode.Success)
                {
                    return ErrorResult(result.Message);
                }
            }
            await blogRepository.DeleteBlogAsync(blogId);
            return ResponseJsonResult<Blog>.Succeeded;
        }

        /// <summary>
        /// 获得所有的个人博客
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<Blog>> FindAllPersonalBlogsAsync(string? userId,string? token=null)
        {
            if(userId == null && token == null)
            {
                throw new Exception("参数不正确，应至少含有userId或Token中的一个");
            }
            string userGuid;
            if (userId == null)
            {
                userGuid = GetCurrentUserId(token!).ToString();
            }
            else
            {
                userGuid = userId;
            }
            return await blogRepository.GetPersonalAllBlogsAsync(userGuid);
        }

        /// <summary>
        /// 根据token获取id
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Guid GetCurrentUserId(string token)
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

        /// <summary>
        /// 获取用户的基本信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns>返回包含用户id、用户昵称、用户头像地址的元组</returns>
        public (string userId, string userName, string? avatarUri) GetUserInfo(string token)
        {
            string userId = "", userName = "", avatarUri = null;
            var payloadClaims = TokenCommen.GetPayloadInfo(token);
            List<Claim> claims = new List<Claim>();
            if (payloadClaims != null && payloadClaims.Any())
            {
                claims = payloadClaims.ToList();
            }
            foreach (var claim in claims)
            {
                if (claim != null && claim.Type == ClaimTypes.NameIdentifier)
                {
                    userId = claim.Value;
                }
                else if (claim != null && claim.Type == ClaimTypes.Name)
                {
                    userName = claim.Value;
                }
                else if (claim != null && claim.Type == ClaimTypes.Uri)
                {
                    avatarUri = claim.Value;
                }
            }
            return (userId, userName, avatarUri);
        }

        private ResponseJsonResult<Blog> ErrorResult(string? msg=null)
        {
            var result = ResponseJsonResult<Blog>.Failed;
            result.Message = msg;
            return result;
        }

    }
}
