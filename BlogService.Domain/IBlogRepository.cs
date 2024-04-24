using ApiJsonResult;
using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public interface IBlogRepository
    {
        Task<Blog?> FindOneByIdWithCommentsAsync(string id);
        Task<Blog?> FindOneByIdNoCommentsAsync(string id);
        Task<IEnumerable<Blog>> GetPersonalAllBlogsAsync(string userId);
        Task<IEnumerable<Blog>> GetRecommendBlogsAsync(int index, int pageSize);
        Task CreateBlogAsync(Blog blog);
        Task<ResponseJsonResult<Blog>> UpdateBlogAsync(Blog blog);
        Task<ResponseJsonResult<Blog>> DeleteBlogAsync(string id);
        /// <summary>
        /// 判断某个博客是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> BlogExistAsync(Guid id);
    }
}
