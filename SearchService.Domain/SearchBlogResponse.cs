using System.Reflection.Metadata.Ecma335;

namespace SearchService.Domain
{
    //搜索返回结果
    public class SearchBlogResponse()
    {
        private long totalCount;
        public IEnumerable<BlogRecord> Blogs {  get; set; } = new List<BlogRecord>();
        public long TotalCount
        {
            get => totalCount;
            set => Blogs.Count();
        }
        public SearchBlogResponse(IEnumerable<BlogRecord> blogs) : this()
        {
            Blogs = blogs;
            totalCount = blogs.Count();
        }
    };
}
