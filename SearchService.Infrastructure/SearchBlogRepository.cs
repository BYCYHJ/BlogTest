using Nest;
using SearchService.Domain;

namespace SearchService.Infrastructure
{
    public class SearchBlogRepository : ISearchRepository
    {
        private readonly IElasticClient esClient;
        public SearchBlogRepository(IElasticClient esClient)
        {
            this.esClient = esClient;
        }

        //删除
        public Task DeleteAsync(Guid blogId)
        {
            DocumentPath<BlogRecord> deletePath = new DocumentPath<BlogRecord>(blogId);
            return esClient.DeleteAsync(deletePath,d => d.Index("Blogs"));
        }

        public Task SearchBlogAsync(string keyword, int index, int pageSize)
        {
            //esClient.SearchAsync()
        }

        //插入及更新
        public async Task UpsertAsync(BlogRecord blog)
        {
            var response = await esClient.UpdateAsync<BlogRecord>(blog.guid, u => u
            .Index("Blogs")
            .Doc(blog)
            .DocAsUpsert()
            );
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
        }
    }
}
 