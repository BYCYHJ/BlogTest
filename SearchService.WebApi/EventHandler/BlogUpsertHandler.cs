using ApiJsonResult;
using BlogRabbitHelper;
using Newtonsoft.Json;
using SearchService.Domain;

namespace SearchService.WebApi.EventHandler
{
    [EventName("Blog.Create")]
    [EventName("Blog.Update")]
    public class BlogUpsertHandler : JsonEventHandler<BlogMessage>
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration configuration;//用于获得appsetting中配置信息

        public BlogUpsertHandler(ISearchRepository searchRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _searchRepository = searchRepository;
            _httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public override async Task JsonHandle(string eventName, BlogMessage? eventData)
        {
            if (eventData == null)
            {
               return;
            }
            //根据传递的消息信息上传到elasticsearch
            BlogRecord blog = new BlogRecord(
                new Guid(eventData.id),
                eventData.blogTitle,
                eventData.blogContent,
                eventData.previewPhoto,
                eventData.userId
                );
            await  _searchRepository.UpsertAsync(blog);
        }
    }
}
