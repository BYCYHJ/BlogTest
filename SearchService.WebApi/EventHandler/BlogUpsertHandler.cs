using BlogRabbitHelper;
using SearchService.Domain;

namespace SearchService.WebApi.EventHandler
{
    [EventName("Blog.Create")]
    [EventName("Blog.Update")]
    public class BlogUpsertHandler : JsonEventHandler<BlogRecord>
    {
        private readonly ISearchRepository _searchRepository;
        public BlogUpsertHandler(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public override Task JsonHandle(string eventName, BlogRecord? eventData)
        {
            if (eventData == null)
            {
               return Task.CompletedTask;
            }
            return _searchRepository.UpsertAsync(eventData!);
        }
    }
}
