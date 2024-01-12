using BlogRabbitHelper;
using SearchService.Domain;

namespace SearchService.WebApi.EventHandler
{
    [EventName("Blog.Delete")]
    public class BlogDeleteHandler : JsonEventHandler<Guid>
    {
        private readonly ISearchRepository _searchRespository;
        public BlogDeleteHandler(ISearchRepository searchRespository)
        {
            _searchRespository = searchRespository;
        }


        public override Task JsonHandle(string eventName, Guid eventData)
        {
            return _searchRespository.DeleteAsync(eventData);
        }
    }
}
