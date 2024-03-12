using ChatService.Domain;
using ChatService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ChatDomainService _domainService;

        public TestController(ChatDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet]
        public async Task RedisToSqlTest()
        {
            Message msgTest = new Message(sendMsg:"test") { 
                Type=MessageType.Text,
                SenderId="aaa",
                RecipientId="bbb"
            };
            string key = "test";
            await _domainService.UpdateToRedis(key, msgTest);
        }
    }
}
