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

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public Task DeleteAsync(Guid blogId)
        {
            DocumentPath<BlogRecord> deletePath = new DocumentPath<BlogRecord>(blogId);
            return esClient.DeleteAsync(deletePath,d => d.Index("blogs"));
        }

        /// <summary>
        /// 根据关键词进行分页查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SearchBlogResponse> SearchBlogAsync(string keyword, int index, int pageSize)
        {
            //寻找title和content中符合keyword的博客，并按照分数降序排序
            var result = await esClient.SearchAsync<BlogRecord>(s=> s
            .Index("blogs")
            .From((index-1)*pageSize)
            .Size(pageSize)
            .Query(
                q => q.Bool(
                    b => b.Should(
                        sh =>sh.Match(
                            m => m
                            .Field(f => f.Title)
                            .Query(keyword)
                            .Fuzziness(Fuzziness.Auto)
                            ),
                        sh => sh.Match(
                            m => m
                            .Field(f => f.Content)
                            .Query(keyword)
                            .Fuzziness(Fuzziness.Auto)
                            )
                        )
                    )
                )
            .Sort(s => s.Descending("_score"))
            );
            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }
            List<BlogRecord> blogs = new List<BlogRecord>();
            //对于内容，只返回前100个字
            blogs.AddRange(result.Hits.Select(s => {
                var plainContent = s.Source.Content.Length > 100 ? s.Source.Content.Substring(0, 99) : s.Source.Content;
                return new BlogRecord(s.Source.guid, s.Source.Title, plainContent);
                }
            ).ToList());
            var response = new SearchBlogResponse(blogs,result.Total);
            return response;
        }

        /// <summary>
        /// 插入及更新
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpsertAsync(BlogRecord blog)
        {
            var response = await esClient.UpdateAsync<BlogRecord>(blog.guid, u => u
            .Index("blogs")
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
 