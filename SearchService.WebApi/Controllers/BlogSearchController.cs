using ApiJsonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchService.Domain;

namespace SearchService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlogSearchController : ApiBaseController
    {
        private readonly ISearchRepository _searchRepository;
        public BlogSearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        [HttpGet]
        public async Task<ResponseJsonResult<SearchBlogResponse>?> GetBlogsWithKeyword(string keyword,int index,int pageSize)
        {
            return await _searchRepository.SearchBlogAsync(keyword, index, pageSize);
        }
    }
}
