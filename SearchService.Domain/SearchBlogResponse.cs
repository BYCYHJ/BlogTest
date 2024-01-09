namespace SearchService.Domain
{
    //搜索返回结果
    public record SearchBlogResponse(IEnumerable<BlogRecord> Blogs,int TotalCount);
}
